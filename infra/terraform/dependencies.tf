locals {
  dependencies_dns_name = "dependencies.orderflow.internal"

  dependencies_ingress = {
    api_sql = {
      security_group_id = aws_security_group.api.id
      port              = 1433
    }

    api_redis = {
      security_group_id = aws_security_group.api.id
      port              = 6379
    }

    grpc_sql = {
      security_group_id = aws_security_group.grpc.id
      port              = 1433
    }

    grpc_redis = {
      security_group_id = aws_security_group.grpc.id
      port              = 6379
    }

    worker_sql = {
      security_group_id = aws_security_group.worker.id
      port              = 1433
    }

    worker_redis = {
      security_group_id = aws_security_group.worker.id
      port              = 6379
    }

    worker_kafka = {
      security_group_id = aws_security_group.worker.id
      port              = 9092
    }
  }
}

resource "aws_security_group" "dependencies" {
  name        = "orderflow-${var.environment}-dependencies-sg"
  description = "Controls access to temporary OrderFlow dependencies"
  vpc_id      = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-dependencies-sg"
    Component = "Dependencies"
  })
}

resource "aws_vpc_security_group_ingress_rule" "dependencies_from_application" {
  for_each = local.dependencies_ingress

  security_group_id = aws_security_group.dependencies.id

  description                  = "Allows ${each.key} traffic"
  ip_protocol                  = "tcp"
  from_port                    = each.value.port
  to_port                      = each.value.port
  referenced_security_group_id = each.value.security_group_id
}

resource "aws_vpc_security_group_egress_rule" "dependencies_outbound" {
  security_group_id = aws_security_group.dependencies.id

  description = "Allows dependency containers to access external registries"
  ip_protocol = "-1"
  cidr_ipv4   = "0.0.0.0/0"
}

resource "aws_service_discovery_private_dns_namespace" "dependencies" {
  count = var.deploy_dependencies ? 1 : 0

  name        = "orderflow.internal"
  description = "Private DNS namespace for temporary OrderFlow dependencies"
  vpc         = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-dependencies-namespace"
  })
}

resource "aws_service_discovery_service" "dependencies" {
  count = var.deploy_dependencies ? 1 : 0

  name = "dependencies"

  dns_config {
    namespace_id = aws_service_discovery_private_dns_namespace.dependencies[0].id

    dns_records {
      ttl  = 10
      type = "A"
    }

    routing_policy = "MULTIVALUE"
  }

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-dependencies-discovery"
  })
}

resource "aws_ecs_task_definition" "dependencies" {
  family = "orderflow-${var.environment}-dependencies"

  requires_compatibilities = [
    "FARGATE"
  ]

  network_mode = "awsvpc"

  cpu    = 2048
  memory = 8192

  execution_role_arn = aws_iam_role.ecs_task_execution.arn
  task_role_arn      = aws_iam_role.ecs_task.arn

  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }

  container_definitions = jsonencode([
    {
      name      = "sqlserver"
      image     = "mcr.microsoft.com/mssql/server:2022-latest"
      essential = true

      cpu               = 1024
      memoryReservation = 4096

      portMappings = [
        {
          name          = "sqlserver"
          containerPort = 1433
          hostPort      = 1433
          protocol      = "tcp"
        }
      ]

      environment = [
        {
          name  = "ACCEPT_EULA"
          value = "Y"
        },
        {
          name  = "MSSQL_PID"
          value = "Developer"
        }
      ]

      secrets = [
        {
          name      = "MSSQL_SA_PASSWORD"
          valueFrom = local.ecs_secure_parameter_arns["database_password"]
        }
      ]

      healthCheck = {
        command = [
          "CMD-SHELL",
          "SQLCMD=$(find /opt/mssql-tools*/bin/sqlcmd -type f -print -quit); [ -n \"$SQLCMD\" ] && \"$SQLCMD\" -S localhost -U sa -P \"$MSSQL_SA_PASSWORD\" -C -Q \"SELECT 1\""
        ]

        interval    = 30
        timeout     = 10
        retries     = 10
        startPeriod = 90
      }

      logConfiguration = {
        logDriver = "awslogs"

        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs["dependencies"].name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "sqlserver"
        }
      }
    },
    {
      name      = "redis"
      image     = "redis:7-alpine"
      essential = true

      cpu               = 128
      memoryReservation = 256

      portMappings = [
        {
          name          = "redis"
          containerPort = 6379
          hostPort      = 6379
          protocol      = "tcp"
        }
      ]

      command = [
        "redis-server",
        "--appendonly",
        "no"
      ]

      healthCheck = {
        command = [
          "CMD-SHELL",
          "redis-cli ping | grep PONG"
        ]

        interval    = 15
        timeout     = 5
        retries     = 5
        startPeriod = 10
      }

      logConfiguration = {
        logDriver = "awslogs"

        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs["dependencies"].name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "redis"
        }
      }
    },
    {
      name      = "kafka"
      image     = "apache/kafka:4.1.2"
      essential = true

      cpu               = 768
      memoryReservation = 1536

      portMappings = [
        {
          name          = "kafka"
          containerPort = 9092
          hostPort      = 9092
          protocol      = "tcp"
        }
      ]

      environment = [
        {
          name  = "KAFKA_NODE_ID"
          value = "1"
        },
        {
          name  = "KAFKA_PROCESS_ROLES"
          value = "broker,controller"
        },
        {
          name  = "KAFKA_LISTENERS"
          value = "PLAINTEXT://0.0.0.0:9092,CONTROLLER://0.0.0.0:9093"
        },
        {
          name  = "KAFKA_ADVERTISED_LISTENERS"
          value = "PLAINTEXT://${local.dependencies_dns_name}:9092"
        },
        {
          name  = "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP"
          value = "CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT"
        },
        {
          name  = "KAFKA_CONTROLLER_LISTENER_NAMES"
          value = "CONTROLLER"
        },
        {
          name  = "KAFKA_CONTROLLER_QUORUM_VOTERS"
          value = "1@localhost:9093"
        },
        {
          name  = "KAFKA_INTER_BROKER_LISTENER_NAME"
          value = "PLAINTEXT"
        },
        {
          name  = "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR"
          value = "1"
        },
        {
          name  = "KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR"
          value = "1"
        },
        {
          name  = "KAFKA_TRANSACTION_STATE_LOG_MIN_ISR"
          value = "1"
        },
        {
          name  = "KAFKA_DEFAULT_REPLICATION_FACTOR"
          value = "1"
        },
        {
          name  = "KAFKA_MIN_INSYNC_REPLICAS"
          value = "1"
        },
        {
          name  = "KAFKA_AUTO_CREATE_TOPICS_ENABLE"
          value = "false"
        }
      ]

      healthCheck = {
        command = [
          "CMD-SHELL",
          "/opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list >/dev/null 2>&1"
        ]

        interval    = 30
        timeout     = 10
        retries     = 10
        startPeriod = 60
      }

      logConfiguration = {
        logDriver = "awslogs"

        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs["dependencies"].name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "kafka"
        }
      }
    }
  ])

  depends_on = [
    aws_iam_role_policy_attachment.ecs_task_execution,
    aws_iam_role_policy.ecs_execution_parameters
  ]

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-dependencies"
    Component = "Dependencies"
  })
}

resource "aws_ecs_service" "dependencies" {
  count = var.deploy_dependencies ? 1 : 0

  name            = "orderflow-${var.environment}-dependencies"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.dependencies.arn

  desired_count = 1

  launch_type      = "FARGATE"
  platform_version = "LATEST"

  enable_ecs_managed_tags = true
  propagate_tags          = "SERVICE"

  deployment_minimum_healthy_percent = 0
  deployment_maximum_percent         = 100

  network_configuration {
    subnets = [
      for subnet_key, subnet in aws_subnet.main :
      subnet.id
      if startswith(subnet_key, "public_")
    ]

    security_groups = [
      aws_security_group.dependencies.id
    ]

    assign_public_ip = true
  }

  service_registries {
    registry_arn = aws_service_discovery_service.dependencies[0].arn
  }

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-dependencies-service"
    Component = "Dependencies"
  })
}