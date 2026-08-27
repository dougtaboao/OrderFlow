locals {
  ecr_repositories = toset([
    "orderflow-api",
    "orderflow-worker",
    "orderflow-grpc"
  ])
}

resource "aws_ecr_repository" "orderflow" {
  for_each = local.ecr_repositories

  name                 = each.value
  image_tag_mutability = "MUTABLE"
  force_delete         = false

  image_scanning_configuration {
    scan_on_push = true
  }
}