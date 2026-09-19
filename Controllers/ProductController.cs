using ApiEcommerce.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    public ProductController(IProductRepository productRepository, IMapper mapper
    )
    {
        _productRepository = productRepository;
        _mapper = mapper;    
    }
}
