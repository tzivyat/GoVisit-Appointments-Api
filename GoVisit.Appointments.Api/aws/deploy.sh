#!/bin/bash

# AWS Deployment Script for GoVisit Appointments API

# Variables - Replace with your values
ACCOUNT_ID="123456789012"
REGION="us-east-1"
ECR_REPO="govisit-appointments-api"
CLUSTER_NAME="govisit-cluster"
SERVICE_NAME="appointments-api-service"

echo "🚀 Starting AWS deployment..."

# Step 1: Build and push Docker image
echo "📦 Building Docker image..."
cd ../WebApplication1
docker build -t $ECR_REPO .

echo "🔐 Logging into ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com

echo "🏷️ Tagging image..."
docker tag $ECR_REPO:latest $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$ECR_REPO:latest

echo "⬆️ Pushing to ECR..."
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$ECR_REPO:latest

# Step 2: Update ECS service
echo "🔄 Updating ECS service..."
aws ecs update-service \
    --cluster $CLUSTER_NAME \
    --service $SERVICE_NAME \
    --force-new-deployment \
    --region $REGION

echo "✅ Deployment completed!"
echo "🌐 API will be available at: https://your-alb-domain.com/swagger"