# 🔐 Google Login Integration Guide

## 📋 **Tổng quan**

Hệ thống đã được tích hợp đăng nhập với Google OAuth 2.0, cho phép người dùng đăng nhập bằng tài khoản Google của họ.

---

## 🛠️ **Cấu hình Google OAuth**

### **Bước 1: Tạo Google OAuth Client**

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Enable **Google+ API**
4. Vào **Credentials** → **Create Credentials** → **OAuth client ID**
5. Chọn **Application type**: Web application
6. Thêm **Authorized JavaScript origins**:
   ```
   http://localhost:3000
   https://your-frontend-domain.com
   ```
7. Thêm **Authorized redirect URIs** (nếu cần):
   ```
   http://localhost:3000/auth/google/callback
   https://your-frontend-domain.com/auth/google/callback
   ```
8. Copy **Client ID**

### **Bước 2: Cấu hình Backend**

Thêm Google Client ID vào `appsettings.Development.json`:

```json
{
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
  }
}
```

Cho production, thêm vào environment variables trên Render:
```
Google__ClientId=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com
```

---

## 🔌 **API Endpoints**

### **POST /api/auth/google-login**

Đăng nhập bằng Google account.

**Request Body:**
```json
{
  "idToken": "google_id_token_from_frontend"
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "Google login successful",
  "data": {
    "token": "jwt_token_here",
    "userId": 1,
    "email": "user@gmail.com",
    "name": "User Name",
    "role": 4
  },
  "timestamp": "2025-10-29T00:00:00Z"
}
```

**Response (Error - 401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid Google token",
  "timestamp": "2025-10-29T00:00:00Z"
}
```

---

## 🎨 **Frontend Integration**

### **React Example**

```bash
npm install @react-oauth/google
```

```tsx
import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';

function App() {
  const handleGoogleLogin = async (credentialResponse) => {
    try {
      const response = await fetch('http://localhost:5000/api/auth/google-login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          idToken: credentialResponse.credential
        }),
      });

      const data = await response.json();
      
      if (data.success) {
        // Lưu token vào localStorage
        localStorage.setItem('token', data.data.token);
        localStorage.setItem('user', JSON.stringify(data.data));
        
        // Redirect to dashboard
        window.location.href = '/dashboard';
      }
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <GoogleOAuthProvider clientId="YOUR_GOOGLE_CLIENT_ID">
      <GoogleLogin
        onSuccess={handleGoogleLogin}
        onError={() => console.log('Login Failed')}
      />
    </GoogleOAuthProvider>
  );
}
```

### **Vue.js Example**

```bash
npm install vue3-google-login
```

```vue
<template>
  <GoogleLogin :callback="handleGoogleLogin" />
</template>

<script setup>
import { googleTokenLogin } from 'vue3-google-login'

const handleGoogleLogin = async (response) => {
  try {
    const result = await fetch('http://localhost:5000/api/auth/google-login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        idToken: response.credential
      }),
    });

    const data = await result.json();
    
    if (data.success) {
      localStorage.setItem('token', data.data.token);
      localStorage.setItem('user', JSON.stringify(data.data));
      // Redirect...
    }
  } catch (error) {
    console.error('Login failed:', error);
  }
};
</script>
```

---

## 🔐 **Luồng xác thực**

1. **Frontend**: Người dùng click "Login with Google"
2. **Google**: Hiển thị popup chọn tài khoản
3. **Frontend**: Nhận Google ID Token từ Google
4. **Frontend → Backend**: Gửi ID Token đến `/api/auth/google-login`
5. **Backend**: Verify ID Token với Google
6. **Backend**: Kiểm tra user trong database:
   - **Nếu tồn tại**: Lấy thông tin user
   - **Nếu chưa tồn tại**: Tạo user mới với role Member (4)
7. **Backend**: Generate JWT token
8. **Backend → Frontend**: Trả về JWT token + user info
9. **Frontend**: Lưu token và redirect

---

## 📊 **Database Schema**

Khi user đăng nhập lần đầu bằng Google, hệ thống tự động tạo record mới:

```sql
INSERT INTO users (email, name, password_hash, role, created_at)
VALUES (
  'user@gmail.com',
  'User Name',
  'random-guid', -- Password ngẫu nhiên cho OAuth users
  4,             -- Member role
  GETDATE()
);
```

---

## 🧪 **Testing**

### **Test với Postman**

1. Lấy Google ID Token từ [OAuth 2.0 Playground](https://developers.google.com/oauthplayground/)
2. Gửi request:

```http
POST http://localhost:5000/api/auth/google-login
Content-Type: application/json

{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6..."
}
```

### **Test với cURL**

```bash
curl -X POST http://localhost:5000/api/auth/google-login \
  -H "Content-Type: application/json" \
  -d '{"idToken":"YOUR_GOOGLE_ID_TOKEN"}'
```

---

## ⚠️ **Lưu ý bảo mật**

1. ✅ **KHÔNG** commit Google Client ID vào Git
2. ✅ **LUÔN** dùng HTTPS trong production
3. ✅ **XÁC THỰC** Google ID Token ở backend (không tin frontend)
4. ✅ **HẠN CHẾ** Authorized origins chỉ cho domains của bạn
5. ✅ **ĐẶT** JWT expiration time hợp lý (hiện tại: 1 giờ)
6. ✅ **KIỂM TRA** user role trước khi cho phép truy cập resources

---

## 🔄 **CORS Configuration**

Đảm bảo CORS đã được cấu hình trong `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ...

app.UseCors("AllowAll");
```

---

## 📦 **Packages đã cài đặt**

- `Microsoft.AspNetCore.Authentication.Google` (8.0.10)
- `Google.Apis.Auth` (1.72.0)

---

## 🐛 **Troubleshooting**

### **Lỗi: "Invalid Google token"**

- Kiểm tra Google Client ID trong `appsettings.json`
- Đảm bảo ID Token chưa hết hạn (thường 1 giờ)
- Verify rằng frontend dùng đúng Client ID

### **Lỗi: "User email already exists"**

- User đã tồn tại với email đó
- Nếu user đã đăng ký bằng email/password, họ vẫn có thể login bằng Google với cùng email

### **Lỗi CORS**

- Kiểm tra CORS policy trong `Program.cs`
- Thêm frontend origin vào allowed origins

---

## 🚀 **Deployment Checklist**

- [ ] Thêm Google Client ID vào Render environment variables
- [ ] Cấu hình Authorized origins trên Google Cloud Console
- [ ] Test Google login trên production domain
- [ ] Verify CORS cho production frontend
- [ ] Kiểm tra JWT token expiration
- [ ] Setup monitoring cho login failures

---

## 📚 **Tài liệu tham khảo**

- [Google Sign-In for Web](https://developers.google.com/identity/gsi/web/guides/overview)
- [Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
