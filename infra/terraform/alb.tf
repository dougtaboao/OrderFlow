resource "aws_lb" "main" {
  count = var.deploy_runtime ? 1 : 0

  name               = "orderflow-${var.environment}-alb"
  internal           = false
  load_balancer_type = "application"

  security_groups = [
    aws_security_group.alb.id
  ]

  subnets = [
    for subnet_key, subnet in aws_subnet.main :
    subnet.id
    if startswith(subnet_key, "public_")
  ]

  enable_deletion_protection = false

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-alb"
  })
}

resource "aws_lb_target_group" "api" {
  count = var.deploy_runtime ? 1 : 0

  name        = "orderflow-${var.environment}-api"
  port        = 8080
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = aws_vpc.main.id

  deregistration_delay = 30

  health_check {
    enabled             = true
    path                = "/health/live"
    protocol            = "HTTP"
    matcher             = "200"
    interval            = 30
    timeout             = 5
    healthy_threshold   = 2
    unhealthy_threshold = 3
  }

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-api-target-group"
    Component = "api"
  })
}

resource "aws_lb_listener" "http" {
  count = var.deploy_runtime ? 1 : 0

  load_balancer_arn = aws_lb.main[0].arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.api[0].arn
  }

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-http-listener"
  })
}