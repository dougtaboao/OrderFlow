locals {
  ecs_task_definitions = {
    api = {
      repository_name = "orderflow-api"
      cpu             = 256
      memory          = 512
      expose_port     = true
    }

    grpc = {
      repository_name = "orderflow-grpc"
      cpu             = 256
      memory          = 512
      expose_port     = true
    }

    worker = {
      repository_name = "orderflow-worker"
      cpu             = 256
      memory          = 512
      expose_port     = false
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

      environment = [
        {
          name  = "DOTNET_ENVIRONMENT"
          value = "Production"
        },
        {
          name  = "ASPNETCORE_URLS"
          value = "http://+:8080"
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
    aws_iam_role_policy_attachment.ecs_task_execution
  ]

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-${each.key}"
    Component = each.key
  })
}