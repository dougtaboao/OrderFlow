locals {
  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}

resource "aws_vpc" "main" {
  cidr_block = var.vpc_cidr

  enable_dns_support   = true
  enable_dns_hostnames = true

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-vpc"
  })
}

data "aws_availability_zones" "available" {
  state = "available"

  filter {
    name   = "zone-type"
    values = ["availability-zone"]
  }
}

locals {
  subnets = {
    public_a = {
      cidr_block        = cidrsubnet(var.vpc_cidr, 8, 0)
      availability_zone = data.aws_availability_zones.available.names[0]
      public            = true
    }

    public_b = {
      cidr_block        = cidrsubnet(var.vpc_cidr, 8, 1)
      availability_zone = data.aws_availability_zones.available.names[1]
      public            = true
    }

    private_a = {
      cidr_block        = cidrsubnet(var.vpc_cidr, 8, 10)
      availability_zone = data.aws_availability_zones.available.names[0]
      public            = false
    }

    private_b = {
      cidr_block        = cidrsubnet(var.vpc_cidr, 8, 11)
      availability_zone = data.aws_availability_zones.available.names[1]
      public            = false
    }
  }
}

resource "aws_subnet" "main" {
  for_each = local.subnets

  vpc_id                  = aws_vpc.main.id
  cidr_block              = each.value.cidr_block
  availability_zone       = each.value.availability_zone
  map_public_ip_on_launch = each.value.public

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-${replace(each.key, "_", "-")}-subnet"
    Tier = each.value.public ? "Public" : "Private"
  })
}

resource "aws_internet_gateway" "main" {
  vpc_id = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-igw"
  })
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-public-rt"
    Tier = "Public"
  })
}

resource "aws_route" "public_internet" {
  route_table_id         = aws_route_table.public.id
  destination_cidr_block = "0.0.0.0/0"
  gateway_id             = aws_internet_gateway.main.id
}

resource "aws_route_table_association" "public" {
  for_each = {
    for key, subnet in aws_subnet.main :
    key => subnet
    if startswith(key, "public")
  }

  subnet_id      = each.value.id
  route_table_id = aws_route_table.public.id
}

resource "aws_route_table" "private" {
  vpc_id = aws_vpc.main.id

  tags = merge(local.common_tags, {
    Name = "orderflow-${var.environment}-private-rt"
    Tier = "Private"
  })
}

resource "aws_route_table_association" "private" {
  for_each = {
    for key, subnet in aws_subnet.main :
    key => subnet
    if startswith(key, "private")
  }

  subnet_id      = each.value.id
  route_table_id = aws_route_table.private.id
}