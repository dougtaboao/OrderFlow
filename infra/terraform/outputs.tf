output "aws_account_id" {
  description = "AWS account currently authenticated"
  value       = data.aws_caller_identity.current.account_id
}

output "aws_region" {
  description = "AWS region configured for this Terraform project"
  value       = data.aws_region.current.region
}

output "ecr_repository_urls" {
  description = "URLs of the OrderFlow ECR repositories"

  value = {
    for name, repository in aws_ecr_repository.orderflow :
    name => repository.repository_url
  }
}