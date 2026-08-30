data "aws_iam_openid_connect_provider" "github" {
  url = "https://token.actions.githubusercontent.com"
}

data "aws_iam_policy_document" "github_actions_trust" {
  statement {
    effect = "Allow"

    actions = [
      "sts:AssumeRoleWithWebIdentity"
    ]

    principals {
      type = "Federated"

      identifiers = [
        data.aws_iam_openid_connect_provider.github.arn
      ]
    }

    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"

      values = [
        "sts.amazonaws.com"
      ]
    }

    condition {
      test     = "StringLike"
      variable = "token.actions.githubusercontent.com:sub"

      values = [
        "repo:${var.github_owner}/${var.github_repository}:ref:refs/heads/main",
        "repo:${var.github_owner}/${var.github_repository}:ref:refs/heads/feature/*"
      ]
    }
  }
}

resource "aws_iam_role" "github_actions_ecr" {
  name = "OrderFlowGitHubActionsEcrRole"

  description = "Allows OrderFlow GitHub Actions to publish container images to Amazon ECR"

  assume_role_policy = data.aws_iam_policy_document.github_actions_trust.json

  max_session_duration = 3600

  tags = {
    Project   = var.project_name
    ManagedBy = "Manual"
  }
}

data "aws_iam_policy_document" "ecr_push" {
  statement {
    sid    = "GetEcrAuthorizationToken"
    effect = "Allow"

    actions = [
      "ecr:GetAuthorizationToken"
    ]

    resources = ["*"]
  }

  statement {
    sid    = "PushImagesToOrderFlowRepositories"
    effect = "Allow"

    actions = [
      "ecr:BatchCheckLayerAvailability",
      "ecr:CompleteLayerUpload",
      "ecr:InitiateLayerUpload",
      "ecr:PutImage",
      "ecr:UploadLayerPart"
    ]

    resources = [
      for repository in aws_ecr_repository.orderflow :
      repository.arn
    ]
  }
}

resource "aws_iam_role_policy" "ecr_push" {
  name = "OrderFlowEcrPushPolicy"
  role = aws_iam_role.github_actions_ecr.name

  policy = data.aws_iam_policy_document.ecr_push.json
}