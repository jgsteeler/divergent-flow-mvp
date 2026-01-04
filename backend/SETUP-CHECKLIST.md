# MongoDB Setup Checklist - Quick Start

This is your quick reference checklist for setting up MongoDB with the Divergent Flow API.

## Prerequisites Checklist

- [ ] .NET 10 SDK installed
- [ ] MongoDB installed (local) OR MongoDB Atlas account (cloud)
- [ ] Code editor/IDE ready
- [ ] Terminal/command line access

---

## Local MongoDB Setup (Development)

### Step 1: Install MongoDB ☐

Choose your platform:

**macOS:**
```bash
brew tap mongodb/brew
brew install mongodb-community@8.0
brew services start mongodb-community@8.0
```

**Ubuntu/Debian:**
```bash
# Import GPG key
curl -fsSL https://www.mongodb.org/static/pgp/server-8.0.asc | \
   sudo gpg -o /usr/share/keyrings/mongodb-server-8.0.gpg --dearmor

# Add repository
echo "deb [ arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-server-8.0.gpg ] https://repo.mongodb.org/apt/ubuntu $(lsb_release -cs)/mongodb-org/8.0 multiverse" | \
   sudo tee /etc/apt/sources.list.d/mongodb-org-8.0.list

# Install
sudo apt-get update
sudo apt-get install -y mongodb-org
sudo systemctl start mongod
sudo systemctl enable mongod
```

**Docker (Any Platform):**
```bash
docker run -d -p 27017:27017 --name mongodb mongo:8
```

### Step 2: Verify MongoDB is Running ☐

```bash
mongosh
# Should connect successfully
# Type: exit
```

### Step 3: Configure the API ☐

```bash
cd backend
cp .env.example .env
```

Edit `.env` with minimum settings:
```bash
# MongoDB (Required)
MONGODB_CONNECTION_STRING=mongodb://localhost:27017
MONGODB_DATABASE_NAME=divergent_flow

# CORS
CORS_ALLOWED_ORIGINS=http://localhost:5173
```

### Step 4: Build and Run ☐

```bash
cd backend
dotnet build
cd DivergentFlow.Api
dotnet run
```

### Step 5: Verify Setup ☐

Look for these log messages:
```
✓ MongoItemRepository initialized with collection: items
✓ Active IItemRepository implementation: MongoItemRepository
✓ InferenceQueueProcessorService started
```

### Step 6: Test the API ☐

Open browser: http://localhost:5100/swagger

Try creating an item:
```json
POST /api/items
{
  "text": "Test MongoDB setup"
}
```

Expected: `201 Created` response

### Step 7: Verify in MongoDB ☐

```bash
mongosh
use divergent_flow
db.items.find().pretty()
# Should show your test item
```

---

## MongoDB Atlas Setup (Cloud)

### Step 1: Create Atlas Account ☐

- Go to https://www.mongodb.com/cloud/atlas
- Sign up (free M0 tier, no credit card required)
- Create new cluster (choose region closest to you)

### Step 2: Configure Network Access ☐

- Navigate to: Network Access
- Click: "Add IP Address"
- For dev: Click "Allow Access from Anywhere" (0.0.0.0/0)
- ⚠️ For production: Add specific IP addresses

### Step 3: Create Database User ☐

- Navigate to: Database Access
- Click: "Add New Database User"
- Set username and strong password (remember these!)
- Set privileges: "Atlas Admin"
- Click: "Add User"

### Step 4: Get Connection String ☐

- Navigate to: Clusters
- Click: "Connect"
- Choose: "Connect your application"
- Select: "Driver: .NET" and "Version: 2.13 or later"
- Copy connection string (looks like):
  ```
  mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
  ```
- Replace `<password>` with your actual password

### Step 5: Configure the API ☐

```bash
cd backend
cp .env.example .env
```

Edit `.env`:
```bash
MONGODB_CONNECTION_STRING=mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
MONGODB_DATABASE_NAME=divergent_flow
CORS_ALLOWED_ORIGINS=http://localhost:5173
```

### Step 6: Build and Run ☐

```bash
cd backend
dotnet build
cd DivergentFlow.Api
dotnet run
```

### Step 7: Test the API ☐

- Open: http://localhost:5100/swagger
- Try: `POST /api/items` with test data
- Expected: `201 Created` response

### Step 8: Verify in Atlas ☐

- Go to Atlas dashboard
- Click: "Browse Collections"
- Should see: `divergent_flow` database with `items` collection

---

## Optional: Redis Setup (For Projection Cache)

Redis is optional. The API works without it.

### Local Redis ☐

**macOS:**
```bash
brew install redis
brew services start redis
```

**Docker:**
```bash
docker run -d -p 6379:6379 --name redis redis
```

Add to `.env`:
```bash
REDIS_URL=localhost:6379
REDIS_TOKEN=
```

### Upstash Redis (Cloud) ☐

1. Create account at https://upstash.com/
2. Create new Redis database
3. Copy REST URL and token
4. Add to `.env`:
   ```bash
   UPSTASH_REDIS_REST_URL=https://your-db.upstash.io
   UPSTASH_REDIS_REST_TOKEN=your-token
   ```

---

## Troubleshooting Checklist

### MongoDB Connection Issues ☐

- [ ] MongoDB is running: `mongosh` (should connect)
- [ ] Connection string is correct in `.env`
- [ ] No typos in environment variable names
- [ ] For Atlas: Check network access allows your IP
- [ ] For Atlas: Username/password are correct

### API Startup Issues ☐

- [ ] .NET 10 SDK installed: `dotnet --version`
- [ ] All packages restored: `dotnet restore`
- [ ] Build succeeds: `dotnet build`
- [ ] `.env` file exists in `backend/` directory
- [ ] MongoDB connection string starts with `mongodb://` or `mongodb+srv://`

### Inference Not Working ☐

- [ ] Check logs for `InferenceQueueProcessorService started`
- [ ] Item was created successfully (201 response)
- [ ] Wait a few seconds (inference is asynchronous)
- [ ] Check MongoDB for updated item with `inferredType`

### Redis Warnings ☐

- [ ] Redis is optional - warnings are OK
- [ ] To use Redis: Follow Redis setup steps above
- [ ] To ignore: Just leave Redis unconfigured

---

## Success Criteria

You're done when:

✅ MongoDB is running
✅ API starts without errors
✅ Log shows MongoDB repository initialized
✅ POST /api/items creates item successfully
✅ Item appears in MongoDB
✅ GET /api/items returns the item
✅ Background inference processes the item
✅ Item updates with inferred type

---

## Next Steps

Once setup is complete:

1. ✅ Read: `IMPLEMENTATION-SUMMARY.md` for architecture details
2. ✅ Read: `README.md` for full API documentation
3. ✅ Test: All API endpoints using Swagger UI
4. ✅ Develop: Start building features with MongoDB persistence
5. ⏭️ Deploy: Follow deployment guide when ready for staging/production

---

## Quick Reference

| What | Where |
|------|-------|
| **Complete Setup Guide** | `MONGODB-SETUP.md` |
| **Architecture Details** | `IMPLEMENTATION-SUMMARY.md` |
| **API Documentation** | `README.md` |
| **Configuration Template** | `.env.example` |
| **Swagger UI** | http://localhost:5100/swagger |
| **MongoDB Shell** | `mongosh` |

---

## Support

If you encounter issues not covered in this checklist:

1. Check `MONGODB-SETUP.md` for detailed troubleshooting
2. Review `IMPLEMENTATION-SUMMARY.md` for architecture questions
3. Check API logs for specific error messages
4. Verify all environment variables are set correctly

---

**That's it! MongoDB persistence is ready to use.** 🎉
