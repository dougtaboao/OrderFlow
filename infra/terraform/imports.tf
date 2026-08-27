import {
  for_each = local.ecr_repositories

  to = aws_ecr_repository.orderflow[each.value]
  id = each.value
}