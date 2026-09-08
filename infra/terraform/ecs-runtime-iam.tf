data "aws_iam_policy_document" "ecs_execution_parameters" {
  statement {
    sid    = "ReadOrderFlowSecureParameters"
    effect = "Allow"

    actions = [
      "ssm:GetParameters"
    ]

    resources = values(local.ecs_secure_parameter_arns)
  }
}

resource "aws_iam_role_policy" "ecs_execution_parameters" {
  name = "OrderFlowEcsParameterStorePolicy"
  role = aws_iam_role.ecs_task_execution.name

  policy = data.aws_iam_policy_document.ecs_execution_parameters.json
}

data "aws_iam_policy_document" "ecs_task_sqs" {
  statement {
    sid    = "UseOrderCreatedQueues"
    effect = "Allow"

    actions = [
      "sqs:ChangeMessageVisibility",
      "sqs:DeleteMessage",
      "sqs:GetQueueAttributes",
      "sqs:GetQueueUrl",
      "sqs:ReceiveMessage",
      "sqs:SendMessage"
    ]

    resources = [
      aws_sqs_queue.order_created.arn,
      aws_sqs_queue.order_created_dlq.arn
    ]
  }
}

resource "aws_iam_role_policy" "ecs_task_sqs" {
  name = "OrderFlowEcsSqsPolicy"
  role = aws_iam_role.ecs_task.name

  policy = data.aws_iam_policy_document.ecs_task_sqs.json
}