locals {
  ecs_secure_parameters = {
    database = {
      environment_name = "ConnectionStrings__DefaultConnection"
      path             = "/orderflow/${var.environment}/database/connection-string"
    }

    database_password = {
      environment_name = "MSSQL_SA_PASSWORD"
      path             = "/orderflow/${var.environment}/database/sa-password"
    }

    jwt = {
      environment_name = "Jwt__SecretKey"
      path             = "/orderflow/${var.environment}/jwt/secret-key"
    }

    redis = {
      environment_name = "Redis__ConnectionString"
      path             = "/orderflow/${var.environment}/redis/connection-string"
    }
  }

  ecs_secure_parameter_arns = {
    for key, parameter in local.ecs_secure_parameters :
    key => "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter${parameter.path}"
  }
}