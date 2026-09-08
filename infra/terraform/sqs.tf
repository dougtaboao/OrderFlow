resource "aws_sqs_queue" "order_created_dlq" {
  name = "orderflow-${var.environment}-order-created-dlq"

  message_retention_seconds = 1209600

  sqs_managed_sse_enabled = true

  tags = merge(local.common_tags, {
    Name    = "orderflow-${var.environment}-order-created-dlq"
    Purpose = "DeadLetterQueue"
  })
}

resource "aws_sqs_queue" "order_created" {
  name = "orderflow-${var.environment}-order-created"

  visibility_timeout_seconds = 300
  message_retention_seconds  = 345600
  receive_wait_time_seconds  = 10

  sqs_managed_sse_enabled = true

  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.order_created_dlq.arn
    maxReceiveCount     = 3
  })

  tags = merge(local.common_tags, {
    Name    = "orderflow-${var.environment}-order-created"
    Purpose = "OrderCreatedMessages"
  })
}