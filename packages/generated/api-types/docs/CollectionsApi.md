# .CollectionsApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**cancelCollection**](CollectionsApi.md#cancelCollection) | **POST** /api/collections/{id}/cancel | Cancel a collection
[**completeCollection**](CollectionsApi.md#completeCollection) | **POST** /api/collections/{id}/complete | Mark a collection as Collected
[**createCollection**](CollectionsApi.md#createCollection) | **POST** /api/collections | Create a collection request
[**deleteCollection**](CollectionsApi.md#deleteCollection) | **DELETE** /api/collections/{id} | Delete a collected collection
[**getCollection**](CollectionsApi.md#getCollection) | **GET** /api/collections/{id} | Get collection details
[**listCollections**](CollectionsApi.md#listCollections) | **GET** /api/collections | List operational collections
[**registerIncident**](CollectionsApi.md#registerIncident) | **POST** /api/collections/{id}/incidents | Register an operational incident
[**startCollection**](CollectionsApi.md#startCollection) | **POST** /api/collections/{id}/start | Mark a collection as InProgress


# **cancelCollection**
> CollectionDetailsDto cancelCollection(cancelCollectionRequest)

Cancels an active collection. Cancelled status is terminal and does not return to the operational flow.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiCancelCollectionRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiCancelCollectionRequest = {
  
  id: "id_example",
  
  cancelCollectionRequest: {
    reason: "reason_example",
  },
};

const data = await apiInstance.cancelCollection(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **cancelCollectionRequest** | **CancelCollectionRequest**|  |
 **id** | [**string**] |  | defaults to undefined


### Return type

**CollectionDetailsDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **completeCollection**
> CollectionDetailsDto completeCollection()

Completes an in-progress collection only when a driver and vehicle are assigned.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiCompleteCollectionRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiCompleteCollectionRequest = {
  
  id: "id_example",
};

const data = await apiInstance.completeCollection(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**] |  | defaults to undefined


### Return type

**CollectionDetailsDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **createCollection**
> CollectionDetailsDto createCollection(createCollectionRequest)

Creates a collection with Open status, a unique number, driver and vehicle assignment, and Normal priority when priority is not provided.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiCreateCollectionRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiCreateCollectionRequest = {
  
  createCollectionRequest: {
    customerId: "customerId_example",
    senderName: "senderName_example",
    senderAddress: "senderAddress_example",
    recipientName: "recipientName_example",
    recipientAddress: "recipientAddress_example",
    expectedPickupDate: new Date('1970-01-01').toISOString().split('T')[0];,
    priority: "Normal",
    notes: "notes_example",
    driverId: "driverId_example",
    vehicleId: "vehicleId_example",
  },
};

const data = await apiInstance.createCollection(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **createCollectionRequest** | **CreateCollectionRequest**|  |


### Return type

**CollectionDetailsDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**201** | Created |  -  |
**400** | Bad Request |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **deleteCollection**
> void deleteCollection()

Physically removes a collection and its incidents when status is Collected.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiDeleteCollectionRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiDeleteCollectionRequest = {
  
  id: "id_example",
};

const data = await apiInstance.deleteCollection(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**] |  | defaults to undefined


### Return type

**void**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**204** | No Content |  -  |
**404** | Not Found |  -  |
**409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **getCollection**
> CollectionDetailsDto getCollection()

Returns full collection data, operational assignment, and incidents in chronological order.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiGetCollectionRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiGetCollectionRequest = {
  
  id: "id_example",
};

const data = await apiInstance.getCollection(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**] |  | defaults to undefined


### Return type

**CollectionDetailsDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **listCollections**
> PaginatedCollectionResponseDto listCollections()

Lists collections with filters by status, customer, and date range. Supports remote pagination and returns aggregated metrics.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiListCollectionsRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiListCollectionsRequest = {
  
  page: 1,
  
  pageSize: 1,
  
  status: "Open",
  
  customerId: "customerId_example",
  
  startDate: new Date('1970-01-01').toISOString().split('T')[0];,
  
  endDate: new Date('1970-01-01').toISOString().split('T')[0];,
};

const data = await apiInstance.listCollections(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **page** | [**number**] |  | defaults to undefined
 **pageSize** | [**number**] |  | defaults to undefined
 **status** | [**&#39;Open&#39; | &#39;InProgress&#39; | &#39;Collected&#39; | &#39;Cancelled&#39;**]**Array<&#39;Open&#39; &#124; &#39;InProgress&#39; &#124; &#39;Collected&#39; &#124; &#39;Cancelled&#39;>** |  | (optional) defaults to undefined
 **customerId** | [**string**] |  | (optional) defaults to undefined
 **startDate** | [**string**] |  | (optional) defaults to undefined
 **endDate** | [**string**] |  | (optional) defaults to undefined


### Return type

**PaginatedCollectionResponseDto**

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

# **registerIncident**
> CollectionDetailsDto registerIncident(registerIncidentRequest)

Registers an incident with description, server timestamp, and the authenticated responsible user.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiRegisterIncidentRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiRegisterIncidentRequest = {
  
  id: "id_example",
  
  registerIncidentRequest: {
    description: "description_example",
  },
};

const data = await apiInstance.registerIncident(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **registerIncidentRequest** | **RegisterIncidentRequest**|  |
 **id** | [**string**] |  | defaults to undefined


### Return type

**CollectionDetailsDto**

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

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)

# **startCollection**
> CollectionDetailsDto startCollection()

Advances an Open collection to InProgress only when a driver and vehicle are already assigned.

### Example


```typescript
import { createConfiguration, CollectionsApi } from '';
import type { CollectionsApiStartCollectionRequest } from '';

const configuration = createConfiguration();
const apiInstance = new CollectionsApi(configuration);

const request: CollectionsApiStartCollectionRequest = {
  
  id: "id_example",
};

const data = await apiInstance.startCollection(request);
console.log('API called successfully. Returned data:', data);
```


### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**] |  | defaults to undefined


### Return type

**CollectionDetailsDto**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/problem+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
**200** | OK |  -  |
**409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](README.md#documentation-for-api-endpoints) [[Back to Model list]](README.md#documentation-for-models) [[Back to README]](README.md)


