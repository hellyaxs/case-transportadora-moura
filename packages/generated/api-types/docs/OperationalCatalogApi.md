# .OperationalCatalogApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**apiCustomersGet**](OperationalCatalogApi.md#apiCustomersGet) | **GET** /api/customers | List customers available for collections
[**apiDriversGet**](OperationalCatalogApi.md#apiDriversGet) | **GET** /api/drivers | List drivers available for assignment
[**apiVehiclesGet**](OperationalCatalogApi.md#apiVehiclesGet) | **GET** /api/vehicles | List vehicles available for assignment


# **apiCustomersGet**
> Array<OptionDto> apiCustomersGet()


### Example


```typescript
import { createConfiguration, OperationalCatalogApi } from '';

const configuration = createConfiguration();
const apiInstance = new OperationalCatalogApi(configuration);

const request = {};

const data = await apiInstance.apiCustomersGet(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters
This endpoint does not need any parameter.


### Return type

**Array<OptionDto>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **apiDriversGet**
> Array<OptionDto> apiDriversGet()


### Example


```typescript
import { createConfiguration, OperationalCatalogApi } from '';

const configuration = createConfiguration();
const apiInstance = new OperationalCatalogApi(configuration);

const request = {};

const data = await apiInstance.apiDriversGet(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters
This endpoint does not need any parameter.


### Return type

**Array<OptionDto>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **apiVehiclesGet**
> Array<VehicleOptionDto> apiVehiclesGet()


### Example


```typescript
import { createConfiguration, OperationalCatalogApi } from '';

const configuration = createConfiguration();
const apiInstance = new OperationalCatalogApi(configuration);

const request = {};

const data = await apiInstance.apiVehiclesGet(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters
This endpoint does not need any parameter.


### Return type

**Array<VehicleOptionDto>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)


