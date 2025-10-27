# 🚀 Deploy Lab Management API to Render

## 📋 Tổng quan

Render là platform PaaS hỗ trợ deploy Docker container miễn phí với:
- ✅ Free tier (với hạn chế)
- ✅ Auto deploy từ GitHub
- ✅ HTTPS tự động
- ✅ Environment variables management
- ✅ Logs và monitoring

---

## 🎯 Bước 1: Chuẩn bị Code

### **Đã hoàn thành:**
- ✅ Dockerfile đã được cấu hình cho Render (dynamic PORT)
- ✅ render.yaml đã được tạo
- ✅ Secrets đã được tách ra environment variables
- ✅ appsettings.Production.json sẵn sàng

### **Cần làm:**

```bash
# 1. Commit changes
git add .
git commit -m "feat: Configure for Render deployment"

# 2. Push lên GitHub (branch main hoặc dev_deploy)
git push origin dev_deploy
```

---

## 🎯 Bước 2: Setup trên Render

### **Option A: Deploy qua Dashboard (Recommended)**

1. **Đăng nhập Render**: https://dashboard.render.com/

2. **Connect GitHub Repository**:
   - Click **"New +"** → **"Web Service"**
   - Connect your GitHub account
   - Select repository: `Giabaongo/lab-management-backend`
   - Chọn branch: `main` hoặc `dev_deploy`

3. **Configure Service**:
   ```
   Name: lab-management-api
   Region: Singapore (gần nhất với Azure)
   Branch: main (hoặc dev_deploy)
   Runtime: Docker
   Plan: Free (hoặc Starter $7/month)
   ```

4. **Advanced Settings**:
   - **Dockerfile Path**: `./Dockerfile`
   - **Docker Context**: `.`
   - **Health Check Path**: `/api/test-connect`

5. **Environment Variables** (QUAN TRỌNG!):

   Click **"Advanced"** → **"Add Environment Variable"**:

   | Key | Value | Type |
   |-----|-------|------|
   | `ASPNETCORE_ENVIRONMENT` | `Production` | Plain |
   | `ConnectionStrings__DefaultConnection` | `Server=bao-sql-server.database.windows.net;Database=LabManagementDB_v2;User Id=giabaongo;Password=YOUR_NEW_PASSWORD;TrustServerCertificate=False;Trusted_Connection=False;` | **Secret** |
   | `Jwt__Key` | `YOUR_NEW_JWT_KEY_64_CHARS` | **Secret** |
   | `Jwt__Issuer` | `LabManagementAPI` | Plain |
   | `Jwt__Audience` | `LabManagementUsers` | Plain |

   **⚠️ LƯU Ý:**
   - Dùng **NEW PASSWORD** (không phải password cũ đã bị lộ)
   - Mark `ConnectionStrings__DefaultConnection` và `Jwt__Key` là **Secret**

6. **Create Web Service**:
   - Click **"Create Web Service"**
   - Render sẽ tự động build và deploy (~5-10 phút lần đầu)

---

### **Option B: Deploy qua Blueprint (render.yaml)**

1. Đăng nhập Render Dashboard

2. Click **"New +"** → **"Blueprint"**

3. Connect repository và chọn `render.yaml`

4. Render sẽ đọc config từ file

5. **VẪN CẦN** thêm secrets manual:
   - Vào service → Environment
   - Add `ConnectionStrings__DefaultConnection` và `Jwt__Key`

---

## 🎯 Bước 3: Đổi Password (KHẨN CẤP!)

### **Đổi Azure SQL Password:**

```bash
# Option 1: Azure CLI
az sql server update \
  --resource-group <your-resource-group> \
  --name bao-sql-server \
  --admin-password <NEW_STRONG_PASSWORD>

# Option 2: Azure Portal
# 1. Vào Azure Portal
# 2. SQL Server → Security → Reset password
# 3. Nhập password mới (min 8 chars, có uppercase, lowercase, number, special char)
```

### **Generate JWT Key mới:**

```bash
# Generate 64-character random key
openssl rand -base64 64

# Hoặc online: https://generate-random.org/api-key-generator
```

---

## 🎯 Bước 4: Cập nhật Environment Variables trên Render

1. Vào Render Dashboard → Your Service

2. **Environment** tab

3. Update values:
   ```
   ConnectionStrings__DefaultConnection = Server=bao-sql-server.database.windows.net;Database=LabManagementDB_v2;User Id=giabaongo;Password=<NEW_PASSWORD>;TrustServerCertificate=False;Trusted_Connection=False;
   
   Jwt__Key = <NEW_JWT_KEY_64_CHARS>
   ```

4. Click **"Save Changes"**

5. Render sẽ tự động redeploy

---

## 🎯 Bước 5: Verify Deployment

### **Check Deploy Status:**

```
Render Dashboard → Your Service → Logs
```

Logs thành công sẽ hiển thị:
```
✓ Application started
✓ Now listening on: http://[::]:PORT
✓ Hosting environment: Production
```

### **Test API:**

```bash
# Health check
curl https://lab-management-api.onrender.com/api/test-connect

# Swagger UI
https://lab-management-api.onrender.com/swagger

# Test login
curl -X POST https://lab-management-api.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"your-password"}'
```

---

## 🔧 Render Configuration Details

### **Free Plan Limitations:**

- ⏱️ **Sleep after 15 mins** of inactivity (cold start ~30s)
- 💾 **512 MB RAM** (đủ cho .NET API nhỏ)
- 🔄 **750 hours/month** (enough if you have 1 service)
- 🌐 **Free SSL/TLS** certificate
- 📊 **Limited bandwidth** (100GB/month)

### **Starter Plan ($7/month):**

- ✅ **No sleep** - Always on
- 💾 **512 MB RAM**
- 🔄 **Unlimited hours**
- 🌐 **Free SSL/TLS**
- 📊 **More bandwidth** (100GB/month)

### **Regions:**

- 🇺🇸 **Oregon** (US West)
- 🇺🇸 **Ohio** (US East)
- 🇩🇪 **Frankfurt** (EU)
- 🇸🇬 **Singapore** (Asia) ← **Recommended** (gần Azure Southeast Asia)

---

## 📊 Monitoring & Debugging

### **View Logs:**

```
Render Dashboard → Service → Logs (Real-time)
```

### **Common Issues:**

**1. Build failed:**
```bash
# Check Dockerfile path và context
# Ensure LabManagementBackend/ folder exists
```

**2. Cannot connect to database:**
```bash
# Check connection string format
# Ensure Azure SQL firewall allows Render IPs
# Add Render IPs to Azure SQL firewall: 0.0.0.0 - 255.255.255.255 (not recommended for prod)
```

**3. Health check failed:**
```bash
# Ensure /api/test-connect endpoint exists
# Check if app is listening on correct PORT
```

**4. App crashes on startup:**
```bash
# Check environment variables
# View logs for error messages
# Ensure JWT Key is set
```

---

## 🔐 Azure SQL Firewall Configuration

Render sử dụng dynamic IPs, nên cần allow all IPs:

### **Option 1: Allow all IPs (Dễ nhưng kém bảo mật):**

```bash
az sql server firewall-rule create \
  --resource-group <your-rg> \
  --server bao-sql-server \
  --name AllowRender \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 255.255.255.255
```

### **Option 2: Allow Azure Services (Recommended):**

Azure Portal → SQL Server → Networking:
- ✅ Check **"Allow Azure services and resources to access this server"**

---

## 🚀 Auto Deploy on Git Push

Render tự động deploy khi:
- ✅ Push code lên branch đã configure
- ✅ Merge PR vào branch main

Disable auto deploy:
```
Service Settings → Build & Deploy → Auto-Deploy: OFF
```

Manual deploy:
```
Service → Manual Deploy → Deploy Latest Commit
```

---

## 💰 Cost Estimation

| Plan | Price | Best For |
|------|-------|----------|
| **Free** | $0/month | Development, Testing |
| **Starter** | $7/month | Small Production Apps |
| **Standard** | $25/month | Production with more resources |

**Khuyến nghị:**
- Development: **Free plan**
- Production nhỏ: **Starter plan** ($7/month)

---

## 🔄 Update Deployment

### **Deploy new changes:**

```bash
# 1. Make changes
# 2. Commit and push
git add .
git commit -m "feat: your changes"
git push origin main

# 3. Render auto deploys (hoặc manual deploy)
```

### **Rollback:**

```
Render Dashboard → Service → Deploy → Previous Deploys
→ Click "Rollback" on previous successful deploy
```

---

## 📝 Quick Setup Checklist

- [ ] Code đã commit và push lên GitHub
- [ ] Đã đổi Azure SQL password
- [ ] Đã generate JWT key mới
- [ ] Đã tạo service trên Render
- [ ] Đã connect GitHub repository
- [ ] Đã set environment variables (với password MỚI)
- [ ] Đã configure Azure SQL firewall
- [ ] Deploy thành công
- [ ] Test API endpoints
- [ ] Swagger UI accessible
- [ ] Health check pass

---

## 🆘 Troubleshooting

### **Build logs shows "Cannot find Dockerfile":**
- Ensure `Dockerfile` is in root
- Check `render.yaml` dockerfilePath: `./Dockerfile`

### **"Connection to database failed":**
```bash
# 1. Check connection string format
# 2. Verify Azure SQL firewall rules
# 3. Test connection from external IP:
sqlcmd -S bao-sql-server.database.windows.net -U giabaongo -P <password> -d LabManagementDB_v2
```

### **"Application failed to start":**
- Check Logs for exact error
- Verify all environment variables are set
- Ensure JWT Key is not empty

### **"502 Bad Gateway":**
- App might be crashing on startup
- Check if PORT is correctly configured
- View startup logs

---

## 🔗 Useful Links

- **Render Dashboard**: https://dashboard.render.com/
- **Render Docs**: https://render.com/docs
- **Docker on Render**: https://render.com/docs/docker
- **Environment Variables**: https://render.com/docs/environment-variables
- **Render Status**: https://status.render.com/

---

## 📞 Support

- **Render Community**: https://community.render.com/
- **Render Support**: support@render.com
- **Documentation**: https://render.com/docs

---

**Last Updated**: 2025-10-27  
**Next Review**: When deploy completes
