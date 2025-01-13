# my-component

Root component that does all the module rendering.

<!-- Auto Generated Below -->


## Properties

| Property                | Attribute   | Description                                                  | Type     | Default     |
| ----------------------- | ----------- | ------------------------------------------------------------ | -------- | ----------- |
| `moduleId` _(required)_ | `module-id` | The Dnn module id, required in order to access web services. | `number` | `undefined` |


## Dependencies

### Depends on

- dnn-select
- dnn-chevron
- dnn-collapsible

### Graph
```mermaid
graph TD;
  dnn-security-center --> dnn-select
  dnn-security-center --> dnn-chevron
  dnn-security-center --> dnn-collapsible
  dnn-select --> dnn-fieldset
  style dnn-security-center fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*
