.PHONY: setup
setup:
	docker compose build

.PHONY: build
build:
	docker compose build cautionary-alerts-api

.PHONY: serve
serve:
	docker compose build cautionary-alerts-api && docker compose up cautionary-alerts-api

.PHONY: shell
shell:
	docker compose run cautionary-alerts-api bash

# cautionary-alerts-api-test requires further setup of environment variables
.PHONY: test
test:
	docker compose up test-database & docker compose up localstack & docker compose build cautionary-alerts-api-test && docker compose up cautionary-alerts-api-test

# Run these in order to run tests in the IDE
.PHONY: local-test-setup
local-test-setup:
	docker compose up test-database & docker compose up localstack

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: restart-db
restart-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database
	docker compose up -d test-database
