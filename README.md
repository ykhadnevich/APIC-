## Create API's with TDD tests

[![API](https://github.com/ykhadnevich/APIC-/actions/workflows/API.yml/badge.svg)](https://github.com/ykhadnevich/APIC-/actions/workflows/API.yml)

### How to run
- Clone this repository
- Run `dotnet publish -o ./publish`
- Start API server with `dotnet ./publish/APICSharp.dll`
- Server will listen on port 8080

Use following URL's to test the API:

### URL's for testing:
- http://localhost:8080/api/stats/users?date=15.10.2023%2015:18:39
- http://localhost:8080/api/stats/users?date2=15.10.2024%2015:18:39
- http://localhost:8080/api/stats/user?date=12.10.2023%2011:03:25&userId=8b0b5db6-19d6-d777-575e-915c2a77959a
- http://localhost:8080/api/predictions/user?date2=15.12.2024%2015:18:39&tolerance=0.4&userId=2fba2529-c166-8574-2da2-eac544d82634
- http://localhost:8080/api/stats/user/total?userId=5ed4eae5-d93c-6b18-be47-93a787c73bcb
- http://localhost:8080/api/stats/user/total?userId1=5ed4eae5-d93c-6b18-be47-93a787c73bcb
- http://localhost:8080/api/user/forget?forgetID=2fba2529-c166-8574-2da2-eac544d82634
