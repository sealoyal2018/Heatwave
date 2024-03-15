using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heatwave.Application.Common;
public abstract record ApiResult(string Msg, int Code);

public record SuccessResult<T>(T Data, string Msg = "操作成功") : ApiResult(Msg, 200);

public record FailResult(string Msg, int Code) : ApiResult(Msg, Code);
