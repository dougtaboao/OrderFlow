import {
  for_each = local.ecr_repositories

  to = aws_ecr_repository.orderflow[each.value]
  id = each.value
}

import {
  to = aws_iam_role.github_actions_ecr
  id = "OrderFlowGitHubActionsEcrRole"
}

import {
  to = aws_iam_role_policy.ecr_push
  id = "OrderFlowGitHubActionsEcrRole:OrderFlowEcrPushPolicy"
}