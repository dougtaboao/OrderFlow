resource "aws_ecs_cluster" "main" {
  name = "orderflow-${var.environment}-cluster"

  setting {
    name  = "containerInsights"
    value = "disabled"
  }

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-cluster"
  })
}

locals {
  ecs_log_groups = toset([
    "api",
    "grpc",
    "worker"
  ])
}

resource "aws_cloudwatch_log_group" "ecs" {
  for_each = local.ecs_log_groups

  name              = "/ecs/orderflow-${var.environment}/${each.value}"
  retention_in_days = 7

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-${each.value}-logs"
    Component = each.value
  })
}