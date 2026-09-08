locals {
  ecs_task_definitions = {
    api = {
      repository_name = "orderflow-api"
      cpu             = 256
      memory          = 512
      expose_port     = true
      environment = [
        {
          name  = "Jwt__Issuer"
          value = "OrderFlow"
        },
        {
          name  = "Jwt__Audience"
          value = "OrderFlow.Api"
        },
        {
          name  = "RabbitMq__QueueName"
          value = "order-created"
        },
        {
          name  = "RabbitMq__DeadLetterQueueName"
          value = "order-created-dlq"
        },
        {
          name  = "RabbitMq__MaxRetryCount"
          value = "3"
        },
        {
          name  = "Kafka__OrderCompletedTopic"
          value = "order-completed"
        },
        {
          name  = "Kafka__OrderStatusChangedTopic"
          value = "order-status-changed"
        },
        {
          name  = "Redis__OrderCacheExpirationMinutes"
          value = "5"
        }
      ]
      secure_parameters = [
        "database",
        "jwt",
        "redis"
      ]
    }

    grpc = {
      repository_name = "orderflow-grpc"
      cpu             = 256
      memory          = 512
      expose_port     = true
      environment = [
        {
          name  = "Kafka__OrderCompletedTopic"
          value = "order-completed"
        },
        {
          name  = "Redis__OrderCacheExpirationMinutes"
          value = "5"
        }
      ]
      secure_parameters = [
        "database",
        "redis"
      ]
    }

    worker = {
      repository_name = "orderflow-worker"
      cpu             = 256
      memory          = 512
      expose_port     = false
      secure_parameters = [
        "database",
        "redis"
      ]

      environment = [
        {
          name  = "Workers__EnableOrderConsumer"
          value = "true"
        },
        {
          name  = "Workers__EnableOutboxPublisher"
          value = "false"
        },
        {
          name  = "Workers__EnableKafkaAudit"
          value = "false"
        },
        {
          name  = "Messaging__Provider"
          value = "Sqs"
        },
        {
          name  = "Sqs__Enabled"
          value = "true"
        },
        {
          name  = "Sqs__Region"
          value = var.aws_region
        },
        {
          name  = "Sqs__QueueUrl"
          value = aws_sqs_queue.order_created.url
        },
        {
          name  = "Sqs__DeadLetterQueueUrl"
          value = aws_sqs_queue.order_created_dlq.url
        },
        {
          name  = "Sqs__MaxMessages"
          value = "5"
        },
        {
          name  = "Sqs__WaitTimeSeconds"
          value = "10"
        },
        {
          name  = "Kafka__OrderCompletedTopic"
          value = "order-completed"
        },
        {
          name  = "Kafka__OrderStatusChangedTopic"
          value = "order-status-changed"
        },
        {
          name  = "Redis__OrderCacheExpirationMinutes"
          value = "5"
        }
      ]
    }

    outbox = {
      repository_name = "orderflow-worker"
      cpu             = 256
      memory          = 512
      expose_port     = false
      secure_parameters = [
        "database",
        "redis"
      ]

      environment = [
        {
          name  = "Workers__EnableOrderConsumer"
          value = "false"
        },
        {
          name  = "Workers__EnableOutboxPublisher"
          value = "true"
        },
        {
          name  = "Workers__EnableKafkaAudit"
          value = "false"
        },
        {
          name  = "Messaging__Provider"
          value = "Sqs"
        },
        {
          name  = "Sqs__Enabled"
          value = "true"
        },
        {
          name  = "Sqs__Region"
          value = var.aws_region
        },
        {
          name  = "Sqs__QueueUrl"
          value = aws_sqs_queue.order_created.url
        },
        {
          name  = "Sqs__DeadLetterQueueUrl"
          value = aws_sqs_queue.order_created_dlq.url
        },
        {
          name  = "Sqs__MaxMessages"
          value = "5"
        },
        {
          name  = "Sqs__WaitTimeSeconds"
          value = "10"
        },
        {
          name  = "Kafka__OrderCompletedTopic"
          value = "order-completed"
        },
        {
          name  = "Kafka__OrderStatusChangedTopic"
          value = "order-status-changed"
        },
        {
          name  = "Redis__OrderCacheExpirationMinutes"
          value = "5"
        }
      ]
    }

    kafka-audit = {
      repository_name = "orderflow-worker"
      cpu             = 256
      memory          = 512
      expose_port     = false
      secure_parameters = [
        "database",
        "redis"
      ]

      environment = [
        {
          name  = "Workers__EnableOrderConsumer"
          value = "false"
        },
        {
          name  = "Workers__EnableOutboxPublisher"
          value = "false"
        },
        {
          name  = "Workers__EnableKafkaAudit"
          value = "true"
        },
        {
          name  = "Messaging__Provider"
          value = "Sqs"
        },
        {
          name  = "Sqs__Enabled"
          value = "true"
        },
        {
          name  = "Sqs__Region"
          value = var.aws_region
        },
        {
          name  = "Sqs__QueueUrl"
          value = aws_sqs_queue.order_created.url
        },
        {
          name  = "Sqs__DeadLetterQueueUrl"
          value = aws_sqs_queue.order_created_dlq.url
        },
        {
          name  = "Sqs__MaxMessages"
          value = "5"
        },
        {
          name  = "Sqs__WaitTimeSeconds"
          value = "10"
        },
        {
          name  = "Kafka__OrderCompletedTopic"
          value = "order-completed"
        },
        {
          name  = "Kafka__OrderStatusChangedTopic"
          value = "order-status-changed"
        },
        {
          name  = "Redis__OrderCacheExpirationMinutes"
          value = "5"
        }
      ]
    }
  }
}

resource "aws_ecs_task_definition" "orderflow" {
  for_each = local.ecs_task_definitions

  family = "orderflow-${var.environment}-${each.key}"

  requires_compatibilities = [
    "FARGATE"
  ]

  network_mode = "awsvpc"

  cpu    = each.value.cpu
  memory = each.value.memory

  execution_role_arn = aws_iam_role.ecs_task_execution.arn
  task_role_arn      = aws_iam_role.ecs_task.arn

  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }

  container_definitions = jsonencode([
    {
      name = each.key

      image = "${aws_ecr_repository.orderflow[each.value.repository_name].repository_url}:latest"

      essential = true

      portMappings = each.value.expose_port ? [
        {
          name          = "${each.key}-http"
          containerPort = 8080
          hostPort      = 8080
          protocol      = "tcp"
        }
      ] : []

      environment = concat(
        [
          {
            name  = "DOTNET_ENVIRONMENT"
            value = "Production"
          },
          {
            name  = "ASPNETCORE_URLS"
            value = "http://+:8080"
          }
        ],
        each.value.environment
      )

      secrets = [
        for parameter_key in each.value.secure_parameters : {
          name      = local.ecs_secure_parameters[parameter_key].environment_name
          valueFrom = local.ecs_secure_parameter_arns[parameter_key]
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"

        options = {
          awslogs-group         = aws_cloudwatch_log_group.ecs[each.key].name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])

  depends_on = [
    aws_iam_role_policy_attachment.ecs_task_execution,
    aws_iam_role_policy.ecs_execution_parameters
  ]

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-${each.key}"
    Component = each.key
  })
}