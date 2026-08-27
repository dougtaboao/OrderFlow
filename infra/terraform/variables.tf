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