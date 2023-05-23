using EPM.Mouser.Interview.Data;
using EPM.Mouser.Interview.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPM.Mouser.Interview.Web.Controllers
{ 
    public class WarehouseApi : Controller
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseApi(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        /*
         *  Action: GET
         *  Url: api/warehouse/id
         *  This action should return a single product for an Id
         */
        [HttpGet]
        [Route ("api/warehouse/id")]
        public async Task<Product?> GetProduct(long id)
        {
            var product = await _warehouseRepository.Get(id);
            return product;
        }

        /*
         *  Action: GET
         *  Url: api/warehouse
         *  This action should return a collection of products in stock
         *  In stock means In Stock Quantity is greater than zero and In Stock Quantity is greater than the Reserved Quantity
         */
        [HttpGet]
        [Route("api/warehouse")]
        public async Task<List<Product>> GetPublicInStockProducts()
        {
            var listOfProducts = await _warehouseRepository.List();
            var requiredProducts = listOfProducts.Where(x => x.InStockQuantity > 0 && x.InStockQuantity > x.ReservedQuantity).ToList();
            return requiredProducts;
        }

        /*
         *  Action: GET
         *  Url: api/warehouse/order
         *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
         *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
         *       {
         *           "id": 1,
         *           "quantity": 1
         *       }
         *
         *  This action should increase the Reserved Quantity for the product requested by the amount requested
         *
         *  This action should return failure (success = false) when:
         *     - ErrorReason.NotEnoughQuantity when: The quantity being requested would increase the Reserved Quantity to be greater than the In Stock Quantity.
         *     - ErrorReason.QuantityInvalid when: A negative number was requested
         *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        [HttpGet]
        [Route("api/warehouse/order")]
        public async Task<UpdateResponse> OrderItem([FromBody] UpdateQuantityRequest updateQuantityRequest)
        {
            var response = new UpdateResponse()
            {
                Success = false
            };
            
            var product =  await _warehouseRepository.Get(updateQuantityRequest.Id);

            if (product == null)
            {
                response.ErrorReason = ErrorReason.InvalidRequest;
                return response;
            }


            if (updateQuantityRequest.Quantity < 0)
            {
                response.ErrorReason = ErrorReason.QuantityInvalid;
                return response;
            }

            if (updateQuantityRequest.Quantity + product.ReservedQuantity > product.InStockQuantity)
            {
                response.ErrorReason = ErrorReason.NotEnoughQuantity;
                return response;
            }


            product.ReservedQuantity += updateQuantityRequest.Quantity;

            await _warehouseRepository.UpdateQuantities(product);

            response.Success = true;

            return response;
        }

        /*
         *  Url: api/warehouse/ship
         *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
         *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
         *       {
         *           "id": 1,
         *           "quantity": 1
         *       }'
         *
         *
         *  This action should:
         *     - decrease the Reserved Quantity for the product requested by the amount requested to a minimum of zero.
         *     - decrease the In Stock Quantity for the product requested by the amount requested
         *
         *  This action should return failure (success = false) when:
         *     - ErrorReason.NotEnoughQuantity when: The quantity being requested would cause the In Stock Quantity to go below zero.
         *     - ErrorReason.QuantityInvalid when: A negative number was requested
         *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        [HttpGet]
        [Route("api/warehouse/ship")]
        public async Task<UpdateResponse> ShipItem([FromBody] UpdateQuantityRequest updateQuantityRequest)
        {
            var response = new UpdateResponse()
            {
                Success = false
            };

            var product = await _warehouseRepository.Get(updateQuantityRequest.Id);

            if (product == null)
            {
                response.ErrorReason = ErrorReason.InvalidRequest;
                return response;
            }

            if (updateQuantityRequest.Quantity < 0)
            {
                response.ErrorReason = ErrorReason.QuantityInvalid;
                return response;
            }

            if (product.InStockQuantity - updateQuantityRequest.Quantity < 0 )
            {

                response.ErrorReason = ErrorReason.NotEnoughQuantity;
                return response;
            }

            if (product.InStockQuantity > 0)
            {
                product.ReservedQuantity -= updateQuantityRequest.Quantity;

                if (product.ReservedQuantity < 0)
                {
                    product.ReservedQuantity = 0;
                }
                
            }
            product.InStockQuantity -= updateQuantityRequest.Quantity;

            await _warehouseRepository.UpdateQuantities(product);

            response.Success = true;
            return response;
        }

        /*
        *  Url: api/warehouse/restock
        *  This action should return a EPM.Mouser.Interview.Models.UpdateResponse
        *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.UpdateQuantityRequest in JSON format in the body of the request
        *       {
        *           "id": 1,
        *           "quantity": 1
        *       }
        *
        *
        *  This action should:
        *     - increase the In Stock Quantity for the product requested by the amount requested
        *
        *  This action should return failure (success = false) when:
        *     - ErrorReason.QuantityInvalid when: A negative number was requested
        *     - ErrorReason.InvalidRequest when: A product for the id does not exist
        */
        [HttpGet]
        [Route("api/warehouse/restock")]
        public async Task<UpdateResponse> RestockItem([FromBody] UpdateQuantityRequest updateQuantityRequest)
        {
            var response = new UpdateResponse()
            {
                Success = false
            };

            var product = await _warehouseRepository.Get(updateQuantityRequest.Id);

            if (product == null)
            {
                response.ErrorReason = ErrorReason.InvalidRequest;
                return response;
            }

            if (updateQuantityRequest.Quantity < 0)
            {
                response.ErrorReason = ErrorReason.QuantityInvalid;
                return response;
            }

            product.InStockQuantity += updateQuantityRequest.Quantity;

            response.Success = true;
            return response;
        }

        /*
        *  Url: api/warehouse/add
        *  This action should return a EPM.Mouser.Interview.Models.CreateResponse<EPM.Mouser.Interview.Models.Product>
        *  This action should have handle an input parameter of EPM.Mouser.Interview.Models.Product in JSON format in the body of the request
        *       {
        *           "id": 1,
        *           "inStockQuantity": 1,
        *           "reservedQuantity": 1,
        *           "name": "product name"
        *       }
        *
        *
        *  This action should:
        *     - create a new product with:
        *          - The requested name - But forced to be unique - see below
        *          - The requested In Stock Quantity
        *          - The Reserved Quantity should be zero
        *
        *       UNIQUE Name requirements
        *          - No two products can have the same name
        *          - Names should have no leading or trailing whitespace before checking for uniqueness
        *          - If a new name is not unique then append "(x)" to the name [like windows file system does, where x is the next avaiable number]
        *
        *
        *  This action should return failure (success = false) and an empty Model property when:
        *     - ErrorReason.QuantityInvalid when: A negative number was requested for the In Stock Quantity
        *     - ErrorReason.InvalidRequest when: A blank or empty name is requested
        */
        [HttpGet]
        [Route("api/warehouse/add")]
        public async Task<UpdateResponse> AddNewProduct([FromBody] Product product)
        {
            var response = new UpdateResponse()
            {
                Success = false
            };

            if (product.InStockQuantity < 0)
            {

                response.ErrorReason = ErrorReason.NotEnoughQuantity;
                return response;
            }

            if (string.IsNullOrEmpty(product.Name))
            {
                response.ErrorReason = ErrorReason.InvalidRequest;
                return response;
            }

            var ProductList = await _warehouseRepository.List();

            var trimmedProductName = product.Name.Trim();

            if (ProductList.Any(x => x.Name == trimmedProductName))
            {
                product.Name = $"x_{trimmedProductName}";
            }
            else
            {
                product.Name = trimmedProductName;
            }

            product.ReservedQuantity = 0;

            await _warehouseRepository.Insert(product);

            response.Success = true;

            return response;
        }
    }
}
