﻿using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace TimeZoneConverter;

[AttributeUsage(AttributeTargets.Method)]
public class ResponseSerializerAttribute : Attribute, IOperationBehavior
{
    private IOperationBehavior? XmlSerializerBehavior { get; set; }

    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
        if (XmlSerializerBehavior != null)
        {
            XmlSerializerBehavior.AddBindingParameters(operationDescription, bindingParameters);
        }
    }

    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
        throw new NotImplementedException();
    }

    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
        XmlSerializerBehavior = operationDescription.OperationBehaviors[typeof(XmlSerializerOperationBehavior)];

        if (XmlSerializerBehavior != null && dispatchOperation.Formatter == null)
        {
            // no formatter has been applied yet - let WCF initialize the formatter with the default behavior
            XmlSerializerBehavior.ApplyDispatchBehavior(operationDescription, dispatchOperation);
        }

        // Override the default formatter, but pass in the default formatter that was initialized above so both can be used
        dispatchOperation.Formatter = new ResponseDispatchFormatter(dispatchOperation.Formatter);
    }

    public void Validate(OperationDescription operationDescription)
    {
        if (XmlSerializerBehavior != null)
        {
            XmlSerializerBehavior.Validate(operationDescription);
        }
    }
}