using BioWings.Application.Features.Results.UserResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.UserQueries;
public class UserGetQuery : IRequest<ServiceResult<List<UserGetQueryResult>>>
{//parametre almadığı için buraya birşey yazmıyoruz.
}
