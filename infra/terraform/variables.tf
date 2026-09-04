variable "aws_region" {
  description = "AWS region used by the OrderFlow infrastructure"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Project name used for resource identification"
  type        = string
  default     = "OrderFlow"
}

variable "github_owner" {
  description = "GitHub account that owns the OrderFlow repository"
  type        = string
  default     = "dougtaboao"
}

variable "github_repository" {
  description = "GitHub repository authorized to access AWS through OIDC"
  type        = string
  default     = "OrderFlow"
}

variable "environment" {
  description = "Environment represented by this Terraform state"
  type        = string
  default     = "dev"
}

variable "vpc_cidr" {
  description = "IPv4 CIDR block reserved for the OrderFlow VPC"
  type        = string
  default     = "10.0.0.0/16"

  validation {
    condition     = can(cidrnetmask(var.vpc_cidr))
    error_message = "vpc_cidr must be a valid IPv4 CIDR block."
  }
}