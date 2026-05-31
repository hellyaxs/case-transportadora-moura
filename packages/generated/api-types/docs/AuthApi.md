# .AuthApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**getCurrentUser**](AuthApi.md#getCurrentUser) | **GET** /api/auth/me | Get the authenticated user
[**login**](AuthApi.md#login) | **POST** /api/auth/login | Authenticate an operational user
[**logout**](AuthApi.md#logout) | **POST** /api/auth/logout | End the current session


# **getCurrentUser**
> AuthenticatedUserDto getCurrentUser()

Returns the current session user when the HttpOnly cookie is valid.

### Example


```typescript
import { createConfiguration, AuthApi } from '';

const configuration = createConfiguration();
const apiInstance = new AuthApi(configuration);

const request = {};

const data = await apiInstance.getCurrentUser(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters
This endpoint does not need any parameter.


### Return type

**AuthenticatedUserDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**401** | Unauthorized |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **login**
> AuthenticatedUserDto login(loginRequest)

Validates credentials and stores the signed JWT in an HttpOnly cookie.

### Example


```typescript
import { createConfiguration, AuthApi } from '';
import type { AuthApiLoginRequest } from '';

const configuration = createConfiguration();
const apiInstance = new AuthApi(configuration);

const request: AuthApiLoginRequest = {
  
  loginRequest: {
    email: "email_example",
    password: "password_example",
  },
};

const data = await apiInstance.login(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **loginRequest** | **LoginRequest**|  |


### Return type

**AuthenticatedUserDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**400** | Bad Request |  -  |
**401** | Unauthorized |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **logout**
> void logout()

Removes the authentication cookie from the client.

### Example


```typescript
import { createConfiguration, AuthApi } from '';

const configuration = createConfiguration();
const apiInstance = new AuthApi(configuration);

const request = {};

const data = await apiInstance.logout(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters
This endpoint does not need any parameter.


### Return type

**void**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** | No Content |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)


