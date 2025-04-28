uni project

## how to run it (don't recommend doing this):
1. clone the repository:
   ```bash
   git clone https://github.com/hasfory/GiftGenieAPIWebApp.git
   ```
2. change connection string in the file appsettings.json 
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME\\SQLEXPRESS; Database=YOUR_DB_NAME; Trusted_Connection=True; MultipleActiveResultSets=true"
}
```
3. build the project
