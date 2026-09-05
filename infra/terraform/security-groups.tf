resource "aws_security_group" "alb" {
  name        = "orderflow-${var.environment}-alb-sg"
  description = "Controls inbound traffic to the OrderFlow Application Load Balancer"
  vpc_id      = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-alb-sg"
  })
}

resource "aws_security_group" "api" {
  name        = "orderflow-${var.environment}-api-sg"
  description = "Controls traffic to OrderFlow API tasks"
  vpc_id      = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-api-sg"
    Component = "Api"
  })
}

resource "aws_security_group" "grpc" {
  name        = "orderflow-${var.environment}-grpc-sg"
  description = "Controls traffic to OrderFlow gRPC tasks"
  vpc_id      = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-grpc-sg"
    Component = "Grpc"
  })
}

resource "aws_security_group" "worker" {
  name        = "orderflow-${var.environment}-worker-sg"
  description = "Controls traffic from OrderFlow Worker tasks"
  vpc_id      = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name      = "orderflow-${var.environment}-worker-sg"
    Component = "Worker"
  })
}

resource "aws_vpc_security_group_ingress_rule" "alb_http" {
  security_group_id = aws_security_group.alb.id

  description = "Allows public HTTP traffic"
  ip_protocol = "tcp"
  from_port   = 80
  to_port     = 80
  cidr_ipv4   = "0.0.0.0/0"
}

resource "aws_vpc_security_group_egress_rule" "alb_to_api" {
  security_group_id = aws_security_group.alb.id

  description                  = "Allows ALB traffic to API tasks"
  ip_protocol                  = "tcp"
  from_port                    = 8080
  to_port                      = 8080
  referenced_security_group_id = aws_security_group.api.id
}

resource "aws_vpc_security_group_egress_rule" "alb_to_grpc" {
  security_group_id = aws_security_group.alb.id

  description                  = "Allows ALB traffic to gRPC tasks"
  ip_protocol                  = "tcp"
  from_port                    = 8080
  to_port                      = 8080
  referenced_security_group_id = aws_security_group.grpc.id
}

resource "aws_vpc_security_group_ingress_rule" "api_from_alb" {
  security_group_id = aws_security_group.api.id

  description                  = "Allows API traffic only from the ALB"
  ip_protocol                  = "tcp"
  from_port                    = 8080
  to_port                      = 8080
  referenced_security_group_id = aws_security_group.alb.id
}

resource "aws_vpc_security_group_ingress_rule" "grpc_from_alb" {
  security_group_id = aws_security_group.grpc.id

  description                  = "Allows gRPC traffic only from the ALB"
  ip_protocol                  = "tcp"
  from_port                    = 8080
  to_port                      = 8080
  referenced_security_group_id = aws_security_group.alb.id
}

resource "aws_vpc_security_group_egress_rule" "application_outbound" {
  for_each = {
    api    = aws_security_group.api.id
    grpc   = aws_security_group.grpc.id
    worker = aws_security_group.worker.id
  }

  security_group_id = each.value

  description = "Allows outbound traffic from OrderFlow application tasks"
  ip_protocol = "-1"
  cidr_ipv4   = "0.0.0.0/0"
}