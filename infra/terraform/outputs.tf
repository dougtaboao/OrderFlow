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

output "public_subnet_ids" {
  description = "IDs of the public subnets"
  value = [
    for key, subnet in aws_subnet.main :
    subnet.id if startswith(key, "public")
  ]
}

output "private_subnet_ids" {
  description = "IDs of the private subnets"
  value = [
    for key, subnet in aws_subnet.main :
    subnet.id if startswith(key, "private")
  ]
}

output "internet_gateway_id" {
  description = "ID of the OrderFlow Internet Gateway"
  value       = aws_internet_gateway.main.id
}

output "public_route_table_id" {
  description = "ID of the public route table"
  value       = aws_route_table.public.id
}

output "private_route_table_id" {
  description = "ID of the private route table"
  value       = aws_route_table.private.id
}

output "security_group_ids" {
  description = "Security Group IDs used by OrderFlow components"

  value = {
    alb    = aws_security_group.alb.id
    api    = aws_security_group.api.id
    grpc   = aws_security_group.grpc.id
    worker = aws_security_group.worker.id
  }
}