using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.FluidApi.Attributes;
using Nop.Plugin.Misc.FluidApi.Delta;
using Nop.Plugin.Misc.FluidApi.DTO.Errors;
using Nop.Plugin.Misc.FluidApi.DTO.OrderItems;
using Nop.Plugin.Misc.FluidApi.DTO.Orders;
using Nop.Plugin.Misc.FluidApi.Factories;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.ActionResults;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.ModelBinders;
using Nop.Plugin.Misc.FluidApi.Models.OrdersParameters;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Nop.Plugin.Misc.FluidApi.Infrastructure.Constants;

namespace Nop.Plugin.Misc.FluidApi.Controllers
{

    public class OrdersController : BaseApiController
    {
        private readonly IOrderApiService _orderApiService;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShippingService _shippingService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IStoreContext _storeContext;
        private readonly Factory<Order> _factory;

        // We resolve the order settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private OrderSettings _orderSettings;

        private OrderSettings OrderSettings => _orderSettings ?? (_orderSettings = EngineContext.Current.Resolve<OrderSettings>());

        public OrdersController(IOrderApiService orderApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductService productService,
            Factory<Order> factory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IShippingService shippingService,
            IPictureService pictureService,
            IDTOHelper dtoHelper,
            IProductAttributeConverter productAttributeConverter)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                 storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _orderApiService = orderApiService;
            _factory = factory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _dtoHelper = dtoHelper;
            _productService = productService;
            _productAttributeConverter = productAttributeConverter;
        }

        /// <summary>
        /// Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrders(OrdersParametersModel parameters)
        {
            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");
            }

            var storeId = _storeContext.GetCurrentStore().Id;

            var orders = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                parameters.CustomerId, storeId);

            IList<OrderDto> ordersAsDtos = (IList<OrderDto>)orders.Select(async x => await _dtoHelper.PrepareOrderDTOAsync(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/count")]
        [ProducesResponseType(typeof(OrdersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersCount(OrdersCountParametersModel parameters)
        {
            var storeId = _storeContext.GetCurrentStore().Id;

            var ordersCount = _orderApiService.GetOrdersCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status,
                                                              parameters.PaymentStatus, parameters.ShippingStatus, parameters.CustomerId, storeId,
                                                              parameters.SinceId);

            var ordersCountRootObject = new OrdersCountRootObject()
            {
                Count = ordersCount
            };

            return Ok(ordersCountRootObject);
        }

        /// <summary>
        /// Retrieve order by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrderByIdAsync(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var order = _orderApiService.GetOrderById(id);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var ordersRootObject = new OrdersRootObject();

            var orderDto = await _dtoHelper.PrepareOrderDTOAsync(order);
            ordersRootObject.Orders.Add(orderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve all orders for customer
        /// </summary>
        /// <param name="customer_id">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customer_id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrdersByCustomerIdAsync(int customer_id)
        {
            IList<OrderDto> ordersForCustomer = (IList<OrderDto>)_orderApiService.GetOrdersByCustomerId(customer_id).Select(async x => await _dtoHelper.PrepareOrderDTOAsync(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }

        /// <summary>
        /// Retrieve all orders for customer pending to deliver.
        /// </summary>
        /// <param name="customer_id">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customer_id}/pending-to-deliver")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersPendingToDeliveryByCustomerId(int customer_id)
        {
            IList<OrderDto> ordersForCustomer = (IList<OrderDto>)_orderApiService.GetOrdersByCustomerId(customer_id)
                                                                .Where(x => x.OrderStatus != OrderStatus.Cancelled && x.ShippingStatus == ShippingStatus.NotYetShipped ||
                                                                       x.ShippingStatus == ShippingStatus.PartiallyShipped ||
                                                                       x.ShippingStatus == ShippingStatus.Shipped)
                                                                .Select(x => _dtoHelper.PrepareOrderDTOAsync(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }

        /// <summary>
        /// Retrieve all orders for customer that were shipped.
        /// </summary>
        /// <param name="customer_id">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customer_id}/delivered")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersShippedByCustomerId(int customer_id)
        {
            IList<OrderDto> ordersForCustomer = (IList<OrderDto>)_orderApiService.GetOrdersByCustomerId(customer_id)
                                                                .Where(x => x.ShippingStatus == ShippingStatus.Delivered)
                                                                .Select(x => _dtoHelper.PrepareOrderDTOAsync(x))
                                                                .ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }

        [HttpPost]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateOrderAsync([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (orderDelta.Dto.CustomerId == null)
            {
                return Error();
            }

            // We doesn't have to check for value because this is done by the order validator.
            var customer = await CustomerService.GetCustomerByIdAsync(orderDelta.Dto.CustomerId.Value);

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            var shippingRequired = false;

            if (orderDelta.Dto.OrderItems != null)
            {
                var shouldReturnError =await AddOrderItemsToCartAsync(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id);
                if (shouldReturnError)
                {
                    return Error(HttpStatusCode.BadRequest);
                }

                shippingRequired = await IsShippingAddressRequiredAsync(orderDelta.Dto.OrderItems);
            }

            if (shippingRequired)
            {
                var isValid = true;

                //isValid &= SetShippingOptionAsync(orderDelta.Dto.ShippingRateComputationMethodSystemName,
                //                             orderDelta.Dto.ShippingMethod,
                //                             orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id,
                //                             customer,
                //                             BuildShoppingCartItemsFromOrderItemDtos(orderDelta.Dto.OrderItems.ToList(),
                //                                                                     customer.Id,
                //                                                                     orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id));

                if (!isValid)
                {
                    return Error(HttpStatusCode.BadRequest);
                }
            }

            var newOrder =await _factory.Initialize();
            orderDelta.Merge(newOrder);

            //customer.BillingAddressId = newOrder.BillingAddressId = orderDelta.Dto.BillingAddress.Id;
            //customer.ShippingAddressId = newOrder.ShippingAddressId = orderDelta.Dto.ShippingAddress.Id;


            // If the customer has something in the cart it will be added too. Should we clear the cart first? 
            newOrder.CustomerId = customer.Id;

            // The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
            if (!orderDelta.Dto.StoreId.HasValue)
            {
                newOrder.StoreId = _storeContext.GetCurrentStore().Id;
            }

            var placeOrderResult = await PlaceOrder(newOrder, customer);

            if (!placeOrderResult.Success)
            {
                foreach (var error in placeOrderResult.Errors)
                {
                    ModelState.AddModelError("order placement", error);
                }

                return Error(HttpStatusCode.BadRequest);
            }

            CustomerActivityService.InsertActivityAsync("AddNewOrder",
                                                  await LocalizationService.GetResourceAsync("ActivityLog.AddNewOrder"), newOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = await _dtoHelper.PrepareOrderDTOAsync(placeOrderResult.PlacedOrder);

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var orderToDelete = _orderApiService.GetOrderById(id);

            if (orderToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            _orderProcessingService.DeleteOrderAsync(orderToDelete);

            //activity log
            CustomerActivityService.InsertActivityAsync("DeleteOrder",await LocalizationService.GetResourceAsync("ActivityLog.DeleteOrder"), orderToDelete);

            return new RawJsonActionResult("{}");
        }

        [HttpPut]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var currentOrder = _orderApiService.GetOrderById(orderDelta.Dto.Id);

            if (currentOrder == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var customer =await CustomerService.GetCustomerByIdAsync(currentOrder.CustomerId);

            //var shippingRequired = _orderService.GetOrderItemsAsync(currentOrder.Id).Any(item => !_productService.GetProductById(item.Id).IsFreeShipping);
            var shippingRequired = false;
            if (shippingRequired)
            {
                var isValid = true;

                if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
                    !string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
                {
                    var storeId = orderDelta.Dto.StoreId ?? _storeContext.GetCurrentStore().Id;

                    //isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
                    //                             orderDelta.Dto.ShippingMethod,
                    //                             storeId,
                    //                             customer, BuildShoppingCartItemsFromOrderItems(_orderService.GetOrderItemsAsync(currentOrder.Id) customer.Id, storeId));
                }

                if (isValid)
                {
                    currentOrder.ShippingMethod = orderDelta.Dto.ShippingMethod;
                }
                else
                {
                    return Error(HttpStatusCode.BadRequest);
                }

            }

            orderDelta.Merge(currentOrder);

            customer.BillingAddressId = currentOrder.BillingAddressId = orderDelta.Dto.BillingAddress.Id;
            customer.ShippingAddressId = currentOrder.ShippingAddressId = orderDelta.Dto.ShippingAddress.Id;


            _orderService.UpdateOrderAsync(currentOrder);

            CustomerActivityService.InsertActivityAsync("UpdateOrder",
                                                   await LocalizationService.GetResourceAsync("ActivityLog.UpdateOrder"), currentOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto =await _dtoHelper.PrepareOrderDTOAsync(currentOrder);
            placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private async Task<bool> SetShippingOptionAsync(string shippingRateComputationMethodSystemName,
            string shippingOptionName,
            int storeId,
            Customer customer,
            List<ShoppingCartItem> shoppingCartItems)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(shippingRateComputationMethodSystemName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_rate_computation_method_system_name",
                                         "Please provide shipping_rate_computation_method_system_name");
            }
            else if (string.IsNullOrEmpty(shippingOptionName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_option_name", "Please provide shipping_option_name");
            }
            else
            {
                var shippingOptionResponse = await _shippingService.GetShippingOptionsAsync(shoppingCartItems, await CustomerService.GetCustomerShippingAddressAsync(customer), customer,
                                                                                 shippingRateComputationMethodSystemName, storeId);

                if (shippingOptionResponse.Success)
                {
                    var shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

                    var shippingOption = shippingOptions
                        .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));

                    _genericAttributeService.SaveAttributeAsync(customer,
                                                           NopCustomerDefaults.SelectedShippingOptionAttribute,
                                                           shippingOption, storeId);
                }
                else
                {
                    isValid = false;

                    foreach (var errorMessage in shippingOptionResponse.Errors)
                    {
                        ModelState.AddModelError("shipping_option", errorMessage);
                    }
                }
            }

            return isValid;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(List<OrderItem> orderItems, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItems)
            {
                shoppingCartItems.Add(new ShoppingCartItem
                {
                    ProductId = orderItem.ProductId,
                    CustomerId = customerId,
                    Quantity = orderItem.Quantity,
                    RentalStartDateUtc = orderItem.RentalStartDateUtc,
                    RentalEndDateUtc = orderItem.RentalEndDateUtc,
                    StoreId = storeId,
                    ShoppingCartType = ShoppingCartType.ShoppingCart
                });
            }

            return shoppingCartItems;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItemDtos(List<OrderItemDto> orderItemDtos, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItemDtos)
            {
                if (orderItem.ProductId != null)
                {
                    shoppingCartItems.Add(new ShoppingCartItem
                    {
                        ProductId = orderItem.ProductId.Value, // required field
                        CustomerId = customerId,
                        Quantity = orderItem.Quantity ?? 1,
                        RentalStartDateUtc = orderItem.RentalStartDateUtc,
                        RentalEndDateUtc = orderItem.RentalEndDateUtc,
                        StoreId = storeId,
                        ShoppingCartType = ShoppingCartType.ShoppingCart
                    });
                }
            }

            return shoppingCartItems;
        }

        private async Task<PlaceOrderResult> PlaceOrder(Order newOrder, Customer customer)
        {
            var processPaymentRequest = new ProcessPaymentRequest
            {
                StoreId = newOrder.StoreId,
                CustomerId = customer.Id,
                PaymentMethodSystemName = newOrder.PaymentMethodSystemName,
                OrderGuid = newOrder.OrderGuid
            };


            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);

            return  placeOrderResult;
        }

        private async Task<bool> IsShippingAddressRequiredAsync(ICollection<OrderItemDto> orderItems)
        {
            var shippingAddressRequired = false;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.ProductId != null)
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId.Value);

                    shippingAddressRequired |= product.IsShipEnabled;
                }
            }

            return shippingAddressRequired;
        }

        private async Task<bool> AddOrderItemsToCartAsync(ICollection<OrderItemDto> orderItems, Customer customer, int storeId)
        {
            var shouldReturnError = false;

            foreach (var orderItem in orderItems)
            {
                if (orderItem.ProductId != null)
                {
                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId.Value);

                    if (!product.IsRental)
                    {
                        orderItem.RentalStartDateUtc = null;
                        orderItem.RentalEndDateUtc = null;
                    }

                    var attributesXml = await _productAttributeConverter.ConvertToXmlAsync(orderItem.Attributes.ToList(), product.Id);

                    var errors = await _shoppingCartService.AddToCartAsync(customer, product,
                        ShoppingCartType.ShoppingCart, storeId, attributesXml,
                        0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                        orderItem.Quantity ?? 1);

                    if (errors.Count() > 0)
                    {
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError("order", error);
                        }

                        shouldReturnError = true;
                    }
                }
            }

            return shouldReturnError;
        }
    }
}