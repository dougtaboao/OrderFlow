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

output "terraform_plan_role_arn" {
  description = "IAM Role assumed by GitHub Actions to run Terraform plan "
  value       = aws_iam_role.terraform_plan.arn
}

output "vpc_id" {
  description = "ID of the OrderFlow VPC"
  value       = aws_vpc.main.id
}

output "vpc_cidr_block" {
  description = "IPv4 CIDR block of the OrderFlow VPC"
  value       = aws_vpc.main.cidr_block
}