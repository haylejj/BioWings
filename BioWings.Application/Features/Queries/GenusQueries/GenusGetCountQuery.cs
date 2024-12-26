using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioWings.Application.Features.Queries.GenusQueries;
public class GenusGetCountQuery : IRequest<ServiceResult<GenusGetCountQueryResult>>
{
}
