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

data "aws_iam_policy_document" "terraform_plan_trust" {
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

locals {
  terraform_state_bucket = "orderflow-terraform-state-${data.aws_caller_identity.current.account_id}-${var.aws_region}"
  terraform_state_key    = "orderflow/dev/terraform.tfstate"
}

resource "aws_iam_role" "terraform_plan" {
  name = "OrderFlowGitHubActionsTerraformPlanRole"

  description = "Allows OrderFlow GitHub Actions to validate and plan Terraform infrastructure"

  assume_role_policy = data.aws_iam_policy_document.terraform_plan_trust.json

  max_session_duration = 3600

  tags = {
    Project   = var.project_name
    ManagedBy = "Terraform"
    Purpose   = "TerraformPlan"
  }
}

data "aws_iam_policy_document" "terraform_plan" {
  statement {
    sid    = "ListTerraformStatePrefix"
    effect = "Allow"

    actions = [
      "s3:ListBucket"
    ]

    resources = [
      "arn:aws:s3:::${local.terraform_state_bucket}"
    ]

    condition {
      test     = "StringLike"
      variable = "s3:prefix"

      values = [
        "orderflow/dev/*"
      ]
    }
  }

  statement {
    sid    = "ReadTerraformState"
    effect = "Allow"

    actions = [
      "s3:GetObject"
    ]

    resources = [
      "arn:aws:s3:::${local.terraform_state_bucket}/${local.terraform_state_key}"
    ]
  }

  statement {
    sid    = "ManageTerraformStateLock"
    effect = "Allow"

    actions = [
      "s3:GetObject",
      "s3:PutObject",
      "s3:DeleteObject"
    ]

    resources = [
      "arn:aws:s3:::${local.terraform_state_bucket}/${local.terraform_state_key}.tflock"
    ]
  }

  statement {
    sid    = "ReadOrderFlowEcrRepositories"
    effect = "Allow"

    actions = [
      "ecr:DescribeRepositories",
      "ecr:ListTagsForResource"
    ]

    resources = ["*"]
  }

  statement {
    sid    = "ListGitHubOidcProviders"
    effect = "Allow"

    actions = [
      "iam:ListOpenIDConnectProviders"
    ]

    resources = ["*"]
  }

  statement {
    sid    = "ReadGitHubOidcProvider"
    effect = "Allow"

    actions = [
      "iam:GetOpenIDConnectProvider"
    ]

    resources = [
      data.aws_iam_openid_connect_provider.github.arn
    ]
  }

  statement {
    sid    = "ReadOrderFlowIamRoles"
    effect = "Allow"

    actions = [
      "iam:GetRole",
      "iam:GetRolePolicy",
      "iam:ListRolePolicies",
      "iam:ListAttachedRolePolicies"
    ]

    resources = [
      aws_iam_role.github_actions_ecr.arn,
      aws_iam_role.terraform_plan.arn
    ]
  }
}

resource "aws_iam_role_policy" "terraform_plan" {
  name = "OrderFlowTerraformPlanReadPolicy"
  role = aws_iam_role.terraform_plan.name

  policy = data.aws_iam_policy_document.terraform_plan.json
}