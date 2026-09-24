using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper
    )
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
        var products = _productRepository.GetProducts();
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }

    [HttpGet("{productId:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProduct(int productId)
    {
        var product = _productRepository.GetProduct(productId);
        if (product is null) NotFound("product not found");
        var productDto = _mapper.Map<ProductDto>(product);

        return Ok(productDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (createProductDto == null) return BadRequest(ModelState);

        if (_productRepository.ProductExists(createProductDto.Name))
        {
            ModelState.AddModelError("CustomError", "Product already exists");
            return BadRequest(ModelState);
        }

        if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", "Category already exists");
            return BadRequest(ModelState);
        }

        var product = _mapper.Map<Product>(createProductDto);
        if (!_productRepository.CreateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong {product.Name}");
            return StatusCode(500, ModelState);
        }
        var createProduct = _productRepository.GetProduct(product.ProductId);
        var productDto = _mapper.Map<ProductDto>(createProduct);
        return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productDto);
    }

    [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsForCategory(int categoryId)
    {
        var products = _productRepository.GetProductsForCategory(categoryId);
        if (products.Count == 0) NotFound("Product not found");
        var productsDto = _mapper.Map<List<ProductDto>>(products);

        return Ok(productsDto);
    }

    [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SearchProducts(string searchTerm)
    {
        var products = _productRepository.SearchProducts(searchTerm);
        if (products.Count == 0) NotFound("Product not found");
        var productsDto = _mapper.Map<List<ProductDto>>(products);

        return Ok(productsDto);
    }

    [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult BuyProduct(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
        {
            return BadRequest("The product name or the quantity is invalid");
        }
        var foundProduct = _productRepository.ProductExists(name);
        if (!foundProduct)
        {
            return NotFound("The product with the name not exists");
        }
        if (!_productRepository.BuyProduct(name, quantity))
        {
            ModelState.AddModelError("CustomError", "The product could be purchased, or the quantity requested exceeds the stock availeble");
            return BadRequest(ModelState);
        }
        return Ok("Units of the product were purchased");
    }

    [HttpPut("{productId:int}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
    {
        if (updateProductDto == null) return BadRequest(ModelState);

        if (!_productRepository.ProductExists(productId))
        {
            ModelState.AddModelError("CustomError", "Product already exists");
            return BadRequest(ModelState);
        }

        if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", "Category already exists");
            return BadRequest(ModelState);
        }

        var product = _mapper.Map<Product>(updateProductDto);
        product.ProductId = productId;
        if (!_productRepository.UpdateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong {product.Name}");
            return StatusCode(500, ModelState);
        }
        return NoContent();
    }


    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteProduct(int productId)
    {
        if(productId == 0) return BadRequest(ModelState);
        var product = _productRepository.GetProduct(productId);
        if (product == null) NotFound("product not found");
        if (!_productRepository.DeleteProduct(product!))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong {product!.Name}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}
