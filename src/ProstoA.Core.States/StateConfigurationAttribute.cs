namespace ProstoA.Core.States;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class)]
public class StateConfigurationAttribute<TState, TConfiguration> : Attribute
    where TState : notnull
    where TConfiguration : StateConfiguration<TState> { }