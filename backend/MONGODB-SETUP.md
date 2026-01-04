# MongoDB Setup Checklist

This document provides step-by-step instructions for setting up MongoDB for the Divergent Flow API.

## Prerequisites

- MongoDB installed locally OR a MongoDB Atlas account
- .NET 10 SDK
- Basic understanding of MongoDB connection strings

## Option 1: Local MongoDB (Development)

### 1. Install MongoDB

#### macOS
```bash
brew tap mongodb/brew
brew install mongodb-community@8.0
brew services start mongodb-community@8.0
```

#### Ubuntu/Debian
```bash
# Import MongoDB GPG key
curl -fsSL https://www.mongodb.org/static/pgp/server-8.0.asc | \
   sudo gpg -o /usr/share/keyrings/mongodb-server-8.0.gpg --dearmor

# Add MongoDB repository
echo "deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-server-8.0.gpg ] https://repo.mongodb.org/apt/ubuntu $(lsb_release -cs)/mongodb-org/8.0 multiverse" | \
   sudo tee /etc/apt/sources.list.d/mongodb-org-8.0.list

# Install MongoDB
sudo apt-get update
sudo apt-get install -y mongodb-org

# Start MongoDB
sudo systemctl start mongod
sudo systemctl enable mongod
```

#### Windows
Download MongoDB Community Server from https://www.mongodb.com/try/download/community

Or use Docker:
```bash
docker run -d -p 27017:27017 --name mongodb mongo:8
```

### 2. Verify MongoDB is Running

```bash
# Connect to MongoDB shell
mongosh

# You should see:
# Current Mongosh Log ID: ...
# Connecting to: mongodb://127.0.0.1:27017/...
# Using MongoDB: ...

# Exit with Ctrl+D or:
exit
```

### 3. Configure the API

#### Option A: Using .env file (Recommended for local development)

1. Copy the example file:
   ```bash
   cd backend
   cp .env.example .env
   ```

2. Edit `.env` and set MongoDB variables (minimum configuration):
   ```bash
   # MongoDB (Required)
   MONGODB_CONNECTION_STRING=mongodb://localhost:27017
   MONGODB_DATABASE_NAME=divergent_flow
   
   # CORS
   CORS_ALLOWED_ORIGINS=http://localhost:5173
   
   # Redis (Optional - comment out if not using)
   # REDIS_URL=localhost:6379
   # REDIS_TOKEN=
   ```

#### Option B: Using environment variables

Set the following environment variables:
```bash
export MONGODB_CONNECTION_STRING=mongodb://localhost:27017
export MONGODB_DATABASE_NAME=divergent_flow
```

Or use the double-underscore notation for nested configuration:
```bash
export MongoDB__ConnectionString=mongodb://localhost:27017
export MongoDB__DatabaseName=divergent_flow
```

### 4. Optional: Customize Collection Names

If you want to use different collection names (default: "items" and "collections"):

```bash
# In .env
MONGODB_ITEMS_COLLECTION=my_items
MONGODB_COLLECTIONS_COLLECTION=my_collections

# Or as environment variables
export MongoDB__ItemsCollectionName=my_items
export MongoDB__CollectionsCollectionName=my_collections
```

## Option 2: MongoDB Atlas (Cloud)

### 1. Create a MongoDB Atlas Account

1. Go to https://www.mongodb.com/cloud/atlas
2. Sign up for a free account (M0 Sandbox cluster - no credit card required)
3. Create a new cluster (choose your preferred region)

### 2. Configure Network Access

1. In Atlas, go to Network Access
2. Click "Add IP Address"
3. For development: Click "Allow Access from Anywhere" (0.0.0.0/0)
   - ⚠️ **WARNING**: This is NOT recommended for production
4. For production: Add your specific IP addresses or CIDR blocks

### 3. Create a Database User

1. In Atlas, go to Database Access
2. Click "Add New Database User"
3. Choose "Password" authentication
4. Set a username and strong password
5. Set Database User Privileges to "Atlas Admin" (or create custom role)
6. Click "Add User"

### 4. Get Connection String

1. In Atlas, go to your cluster
2. Click "Connect"
3. Choose "Connect your application"
4. Select "Driver: .NET" and "Version: 2.13 or later"
5. Copy the connection string (looks like):
   ```
   mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
   ```
6. Replace `<password>` with your actual password
7. Replace `<dbname>` if present with `divergent_flow`

### 5. Configure the API

#### Option A: Using .env file

1. Copy the example file:
   ```bash
   cd backend
   cp .env.example .env
   ```

2. Edit `.env` and set your Atlas connection string:
   ```bash
   MONGODB_CONNECTION_STRING=mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
   MONGODB_DATABASE_NAME=divergent_flow
   ```

#### Option B: Using environment variables

```bash
export MONGODB_CONNECTION_STRING="mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority"
export MONGODB_DATABASE_NAME=divergent_flow
```

## Verify Configuration

### 1. Build the API

```bash
cd backend
dotnet build
```

You should see:
```
Build succeeded.
```

### 2. Run the API

```bash
cd backend/DivergentFlow.Api
dotnet run
```

Look for these log messages on startup:
```
info: MongoDB initialization started
info: MongoItemRepository initialized with collection: items
info: MongoCollectionRepository initialized with collection: collections
info: Active IItemRepository implementation: DivergentFlow.Infrastructure.Repositories.MongoItemRepository
```

If you see errors like:
- `A timeout occurred after 30000ms...` - Check your MongoDB connection string and network access
- `MongoAuthenticationException` - Verify your username/password in the connection string
- `Configuration validation failed` - Check that all required settings are present

### 3. Test the API

Open a browser and navigate to:
```
http://localhost:5100/swagger
```

Try creating an item:
1. Expand "POST /api/items"
2. Click "Try it out"
3. Enter test data:
   ```json
   {
     "text": "Test item from MongoDB"
   }
   ```
4. Click "Execute"
5. You should get a 201 Created response

### 4. Verify Data in MongoDB

#### Local MongoDB
```bash
mongosh
use divergent_flow
db.items.find().pretty()
```

#### MongoDB Atlas
1. In Atlas, go to your cluster
2. Click "Browse Collections"
3. You should see the `divergent_flow` database with `items` and/or `collections`

## Troubleshooting

### Connection Timeout
- **Local**: Ensure MongoDB is running (`brew services list` or `systemctl status mongod`)
- **Atlas**: Check Network Access allows your IP address

### Authentication Failed
- Verify username/password in connection string
- Ensure database user has correct privileges in Atlas
- URL-encode special characters in password (e.g., `@` becomes `%40`)

### Database Not Created
- MongoDB creates databases and collections lazily (on first write)
- Try creating an item via the API, then check again

### "Configuration validation failed"
- Ensure `MONGODB_CONNECTION_STRING` is set
- Ensure `MONGODB_DATABASE_NAME` is set
- Check for typos in environment variable names

### Redis Errors (Optional)
- MongoDB works independently of Redis
- If you don't have Redis configured, you'll see warnings but the API will still work
- The projection writer will use a no-op implementation if Redis is unavailable

## Production Considerations

### Security
- [ ] Use strong passwords for MongoDB users
- [ ] Restrict network access to specific IP addresses
- [ ] Use TLS/SSL for connections
- [ ] Store connection strings in secrets management (Azure Key Vault, AWS Secrets Manager, etc.)
- [ ] Never commit `.env` files to version control

### Connection String Best Practices
- Use `retryWrites=true` for automatic retry of write operations
- Use `w=majority` for write concern (ensures data is replicated)
- Consider connection pooling settings for high-traffic scenarios

### Monitoring
- [ ] Enable MongoDB slow query log
- [ ] Monitor connection pool metrics
- [ ] Set up alerts for failed connections
- [ ] Track query performance in MongoDB Atlas

## Next Steps

Once MongoDB is configured and verified:

1. ✅ MongoDB is running and accessible
2. ✅ API can connect to MongoDB
3. ✅ Items can be created and retrieved
4. ⏭️ Configure Redis for projection cache (optional)
5. ⏭️ Test background inference queue processing
6. ⏭️ Deploy to staging/production environment

## Resources

- [MongoDB Documentation](https://www.mongodb.com/docs/)
- [MongoDB Atlas Documentation](https://www.mongodb.com/docs/atlas/)
- [MongoDB Connection String Format](https://www.mongodb.com/docs/manual/reference/connection-string/)
- [MongoDB .NET Driver](https://www.mongodb.com/docs/drivers/csharp/)
