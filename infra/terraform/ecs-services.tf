locals {
  ecs_services = {
    api = {
      security_group_id = aws_security_group.api.id
    }

    grpc = {
      security_group_id = aws_security_group.grpc.id
    }

    worker = {
      security_group_id = aws_security_group.worker.id
    }

    outbox = {
      security_group_id = aws_security_group.worker.id
    }

    kafka-audit = {
      security_group_id = aws_security_group.worker.id
    }
  }
}

resource "aws_ecs_service" "orderflow" {
  for_each = local.ecs_services

  name            = "orderflow-${var.environment}-${each.key}"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.orderflow[each.key].arn

  desired_count = var.deploy_runtime ? 1 : 0

  launch_type      = "FARGATE"
  platform_version = "LATEST"

  enable_ecs_managed_tags = true
  propagate_tags          = "SERVICE"

  deployment_minimum_healthy_percent = 0
  deployment_maximum_percent         = 200

  deployment_circuit_breaker {
    enable   = true
    rollback = true
  }

  network_configuration {
    subnets = [
      for subnet_key, subnet in aws_subnet.main :
      subnet.id
      if startswith(subnet_key, "public_")
    ]

    security_groups = [
      each.value.security_group_id
    ]

    assign_public_ip = true
  }

  dynamic "load_balancer" {
    for_each = each.key == "api" && var.deploy_runtime ? [1] : []

    content {
      target_group_arn = aws_lb_target_group.api[0].arn
      container_name   = "api"
      container_port   = 8080
    }
  }

  depends_on = [
    aws_lb_listener.http
  ]

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-${each.key}-service"
    Component = each.key
  })
}