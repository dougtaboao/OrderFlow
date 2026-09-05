data "aws_iam_policy_document" "ecs_task_assume_role" {
  statement {
    effect = "Allow"

    actions = [
      "sts:AssumeRole"
    ]

    principals {
      type = "Service"

      identifiers = [
        "ecs-tasks.amazonaws.com"
      ]
    }
  }
}

resource "aws_iam_role" "ecs_task_execution" {
  name = "OrderFlowEcsTaskExecutionRole"

  description = "Allows ECS to pull OrderFlow images and publish container logs"

  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json

  tags = merge(local.common_tags, {
    Name    = "orderflow-${var.environment}-ecs-task-execution-role"
    Purpose = "EcsTaskExecution"
  })
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution" {
  role = aws_iam_role.ecs_task_execution.name

  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role" "ecs_task" {
  name = "OrderFlowEcsTaskRole"

  description = "Identity used by the OrderFlow containers while running in ECS"

  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json

  tags = merge(local.common_tags, {
    Name    = "orderflow-${var.environment}-ecs-task-role"
    Purpose = "EcsApplication"
  })
}