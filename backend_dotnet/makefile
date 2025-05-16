STARTUP_FOLDER=./src/ViberLounge.API
INFRA_FOLDER=./src/ViberLounge.Infrastructure
TEST_FOLDER=./src/ViberLounge.Tests

clean:
	dotnet clean $(STARTUP_FOLDER)

restore:
	dotnet restore $(STARTUP_FOLDER)

build:
	dotnet build $(STARTUP_FOLDER)

test:
	dotnet test $(TEST_FOLDER) 

run:
	dotnet run --project $(STARTUP_FOLDER)

migrate:
	dotnet ef migrations add $(name) --startup-project $(STARTUP_FOLDER) --project $(INFRA_FOLDER)

remove_migration:
	dotnet ef migrations remove --startup-project $(STARTUP_FOLDER) --project $(INFRA_FOLDER)

update_migration:
	dotnet ef database update --startup-project $(STARTUP_FOLDER) --project $(INFRA_FOLDER)

up_dev_db:
	docker compose up -d viber_lounge_database

up_dev_api:
	docker compose -f docker-compose.yml up -d viber_lounge_api