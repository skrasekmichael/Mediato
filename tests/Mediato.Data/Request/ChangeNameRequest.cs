using Mediato.Abstractions;

namespace Mediato.Data.Request;

public sealed record ChangeNameRequest(string NewName) : IRequest<bool>;
public sealed record ChangeNameRequestA(string NewName) : IRequest<bool>;
public sealed record ChangeNameRequestB(string NewName) : IRequest<bool>;
public sealed record ChangeNameRequestC(string NewName) : IRequest<bool>;

