# Mobile App Connection to Render Backend

## ✅ Backend Setup Complete

Backend đã được configure để accept requests từ mobile apps với các settings sau:

### 1. CORS Configuration

**Policy**: `AllowMobile`
```csharp
policy.AllowAnyOrigin()    // Mobile apps có thể request từ bất kỳ origin nào
      .AllowAnyMethod()    // Cho phép GET, POST, PUT, DELETE, etc.
      .AllowAnyHeader();   // Cho phép tất cả headers
```

**Applied in**: `Program.cs` line ~185
```csharp
app.UseCors("AllowMobile");
```

### 2. HTTPS Not Required

```csharp
options.RequireHttpsMetadata = false;  // Mobile có thể dùng HTTP hoặc HTTPS
```

### 3. Authentication

Backend sử dụng **JWT Bearer Token** authentication:
- Token được return sau khi login thành công
- Mobile app cần gửi token trong header: `Authorization: Bearer {token}`

## 📱 Mobile App Configuration

### Render Backend URL

Sau khi deploy lên Render, bạn sẽ có URL dạng:

```
https://your-app-name.onrender.com
```

**Ví dụ**:
```
https://lab-management-api.onrender.com
```

### React Native / Expo Configuration

**File**: `src/config/api.ts` (hoặc tương tự)

```typescript
// api.ts
const IS_PRODUCTION = !__DEV__;

export const API_CONFIG = {
  BASE_URL: IS_PRODUCTION 
    ? 'https://your-app-name.onrender.com/api'  // ← Thay bằng URL Render của bạn
    : 'http://192.168.1.100:5162/api',           // Local dev (thay IP của bạn)
  
  TIMEOUT: 30000, // 30 seconds (Render có thể cold start chậm)
};

export default API_CONFIG.BASE_URL;
```

**Usage**:
```typescript
import axios from 'axios';
import { API_CONFIG } from './config/api';

const api = axios.create({
  baseURL: API_CONFIG.BASE_URL,
  timeout: API_CONFIG.TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
  }
});

// Add auth token interceptor
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken'); // hoặc AsyncStorage
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
```

### Flutter Configuration

**File**: `lib/config/api_config.dart`

```dart
class ApiConfig {
  // Check if running in production mode
  static const bool kReleaseMode = bool.fromEnvironment('dart.vm.product');
  
  // Render backend URL
  static const String productionBaseUrl = 'https://your-app-name.onrender.com/api';
  
  // Local dev URL (thay IP máy của bạn)
  static const String developmentBaseUrl = 'http://192.168.1.100:5162/api';
  
  static String get baseUrl {
    return kReleaseMode ? productionBaseUrl : developmentBaseUrl;
  }
  
  static const int timeoutSeconds = 30; // Render có thể cold start
}
```

**Usage** (`lib/services/api_service.dart`):
```dart
import 'package:dio/dio.dart';
import '../config/api_config.dart';

class ApiService {
  late Dio _dio;
  
  ApiService() {
    _dio = Dio(BaseOptions(
      baseUrl: ApiConfig.baseUrl,
      connectTimeout: Duration(seconds: ApiConfig.timeoutSeconds),
      receiveTimeout: Duration(seconds: ApiConfig.timeoutSeconds),
      headers: {
        'Content-Type': 'application/json',
      },
    ));
    
    // Add auth interceptor
    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await getToken(); // From secure storage
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
    ));
  }
  
  Dio get dio => _dio;
}
```

### Android Native (Kotlin)

**File**: `app/src/main/java/com/example/app/api/ApiConfig.kt`

```kotlin
object ApiConfig {
    private const val PRODUCTION_BASE_URL = "https://your-app-name.onrender.com/api/"
    private const val DEVELOPMENT_BASE_URL = "http://192.168.1.100:5162/api/"
    
    val BASE_URL: String = if (BuildConfig.DEBUG) {
        DEVELOPMENT_BASE_URL
    } else {
        PRODUCTION_BASE_URL
    }
    
    const val TIMEOUT_SECONDS = 30L
}
```

**Retrofit Setup**:
```kotlin
object RetrofitClient {
    private val okHttpClient = OkHttpClient.Builder()
        .connectTimeout(ApiConfig.TIMEOUT_SECONDS, TimeUnit.SECONDS)
        .readTimeout(ApiConfig.TIMEOUT_SECONDS, TimeUnit.SECONDS)
        .writeTimeout(ApiConfig.TIMEOUT_SECONDS, TimeUnit.SECONDS)
        .addInterceptor { chain ->
            val request = chain.request().newBuilder()
                .addHeader("Content-Type", "application/json")
                .apply {
                    // Add auth token if exists
                    getAuthToken()?.let { token ->
                        addHeader("Authorization", "Bearer $token")
                    }
                }
                .build()
            chain.proceed(request)
        }
        .build()
    
    val retrofit: Retrofit = Retrofit.Builder()
        .baseUrl(ApiConfig.BASE_URL)
        .client(okHttpClient)
        .addConverterFactory(GsonConverterFactory.create())
        .build()
}
```

## 🔐 Authentication Flow

### 1. Login Request

**Endpoint**: `POST /api/auth/login`

**Request**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response**:
```json
{
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 123,
      "name": "John Doe",
      "email": "user@example.com",
      "role": 4
    }
  },
  "message": "Login successful",
  "success": true
}
```

### 2. Store Token (React Native Example)

```typescript
import AsyncStorage from '@react-native-async-storage/async-storage';

const login = async (email: string, password: string) => {
  try {
    const response = await api.post('/auth/login', { email, password });
    
    if (response.data.success) {
      const token = response.data.data.token;
      
      // Store token
      await AsyncStorage.setItem('authToken', token);
      
      // Store user info
      await AsyncStorage.setItem('user', JSON.stringify(response.data.data.user));
      
      return response.data.data;
    }
  } catch (error) {
    console.error('Login failed:', error);
    throw error;
  }
};
```

### 3. Use Token in Requests

```typescript
const getLabEvents = async () => {
  try {
    const token = await AsyncStorage.getItem('authToken');
    
    const response = await axios.get(
      `${API_CONFIG.BASE_URL}/labevents`,
      {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      }
    );
    
    return response.data.data;
  } catch (error) {
    console.error('Failed to fetch lab events:', error);
    throw error;
  }
};
```

## ⚠️ Important Notes for Render

### 1. Cold Start Delay

Render free tier "spins down" sau 15 phút không hoạt động. First request sau khi spin down có thể mất **30-60 giây**.

**Solutions**:

**Option A**: Increase timeout in mobile app
```typescript
const api = axios.create({
  timeout: 60000, // 60 seconds for cold start
});
```

**Option B**: Implement retry logic
```typescript
const apiWithRetry = async (config: any, retries = 3) => {
  for (let i = 0; i < retries; i++) {
    try {
      return await api.request(config);
    } catch (error) {
      if (i === retries - 1) throw error;
      await new Promise(resolve => setTimeout(resolve, 2000 * (i + 1)));
    }
  }
};
```

**Option C**: Show loading indicator
```typescript
const [isLoading, setIsLoading] = useState(false);
const [loadingMessage, setLoadingMessage] = useState('');

const fetchData = async () => {
  setIsLoading(true);
  setLoadingMessage('Connecting to server... (may take up to 60s on first load)');
  
  try {
    const data = await api.get('/labevents');
    setLoadingMessage('');
  } catch (error) {
    setLoadingMessage('Connection failed. Please try again.');
  } finally {
    setIsLoading(false);
  }
};
```

### 2. Keep Alive (Prevent Cold Start)

**Option**: Ping backend mỗi 10 phút

```typescript
// React Native
import { AppState } from 'react-native';

useEffect(() => {
  const interval = setInterval(() => {
    if (AppState.currentState === 'active') {
      // Ping backend to keep it warm
      axios.get(`${API_CONFIG.BASE_URL}/test`)
        .catch(() => {}); // Ignore errors
    }
  }, 10 * 60 * 1000); // 10 minutes
  
  return () => clearInterval(interval);
}, []);
```

### 3. HTTPS Only in Production

Render tự động cung cấp HTTPS. Mobile app **NÊN** dùng HTTPS URL:

```
✅ https://your-app-name.onrender.com
❌ http://your-app-name.onrender.com
```

### 4. Check Backend Status

**Health Check Endpoint**: `GET /api/test`

```typescript
const checkBackendStatus = async () => {
  try {
    const response = await axios.get(`${API_CONFIG.BASE_URL}/test`, {
      timeout: 5000
    });
    console.log('Backend is online:', response.data);
    return true;
  } catch (error) {
    console.error('Backend is offline or unreachable');
    return false;
  }
};
```

## 🧪 Testing

### 1. Test from Browser

Mở Swagger UI trên Render:
```
https://your-app-name.onrender.com/swagger
```

Test các endpoints để đảm bảo backend hoạt động.

### 2. Test from Mobile (Dev Mode)

**React Native**:
```bash
# Enable React Native debugging
npx react-native log-android  # Android
npx react-native log-ios      # iOS
```

**Flutter**:
```bash
flutter run --verbose
```

Check console logs để xem requests/responses.

### 3. Test APK on Real Device

1. Build APK/IPA với production config
2. Install trên device thật
3. Test login và các API calls
4. Check network inspector (Charles Proxy, Proxyman)

## 📋 Checklist

Trước khi release mobile app:

- [ ] Backend URL đã update sang Render URL
- [ ] CORS policy = "AllowMobile" đã active
- [ ] Timeout đủ lớn cho cold start (30-60s)
- [ ] JWT token được store và gửi đúng
- [ ] Error handling cho network issues
- [ ] Loading indicator cho slow requests
- [ ] Retry logic implemented
- [ ] HTTPS URL (không phải HTTP)
- [ ] Test trên real device, không chỉ emulator
- [ ] Test khi backend cold (sau 15 phút không dùng)

## 🚀 Deployment Steps

### Backend (Render)

1. **Deploy**: Code đã được push lên Render
2. **Verify**: Check Render dashboard → Service running
3. **Get URL**: Copy URL từ Render dashboard
   - VD: `https://lab-management-api.onrender.com`

### Mobile App

1. **Update Config**:
   ```typescript
   const API_BASE_URL = 'https://your-render-url.onrender.com/api';
   ```

2. **Build**:
   ```bash
   # React Native
   npx react-native run-android --variant=release
   
   # Flutter
   flutter build apk --release
   ```

3. **Test**:
   - Install APK trên device
   - Test login
   - Test API calls
   - Verify token authentication

4. **Distribute**:
   - Upload to Play Store / App Store
   - Or distribute APK directly

## 💡 Common Issues & Solutions

### Issue 1: "Network request failed"

**Cause**: Backend URL sai hoặc backend offline

**Solution**:
```typescript
// Check URL
console.log('API URL:', API_CONFIG.BASE_URL);

// Test connection
const response = await fetch(`${API_CONFIG.BASE_URL}/test`);
console.log('Backend status:', response.status);
```

### Issue 2: "401 Unauthorized"

**Cause**: Token không được gửi hoặc expired

**Solution**:
```typescript
// Check token
const token = await AsyncStorage.getItem('authToken');
console.log('Token:', token ? 'exists' : 'missing');

// Verify token in request
axios.interceptors.request.use(config => {
  console.log('Headers:', config.headers);
  return config;
});
```

### Issue 3: "Timeout exceeded"

**Cause**: Render cold start

**Solution**:
```typescript
// Increase timeout
const api = axios.create({
  timeout: 60000, // 60 seconds
});

// Show message
Alert.alert(
  'Connecting',
  'Backend is waking up, please wait up to 60 seconds...'
);
```

### Issue 4: CORS Error

**Cause**: Backend CORS không config đúng

**Solution**: Đã fix ở backend với policy `AllowMobile`

Verify trong `Program.cs`:
```csharp
app.UseCors("AllowMobile");
```

## 📞 Support

Nếu gặp vấn đề:

1. Check Render logs: Dashboard → Logs tab
2. Check mobile logs: React Native Debugger / Flutter DevTools
3. Verify URL: `https://your-app-name.onrender.com/swagger`
4. Test with Postman first to isolate issue

---

**Backend URL của bạn**: (Copy từ Render dashboard)
```
https://YOUR_RENDER_URL_HERE.onrender.com
```

Update URL này vào mobile app config và rebuild APK! 🚀
