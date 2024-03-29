/* eslint-disable */
/* tslint:disable */
/**
 * This is an autogenerated file created by the Stencil compiler.
 * It contains typing information for all components that exist in this project.
 */
import { HTMLStencilElement, JSXBase } from "@stencil/core/internal";
export namespace Components {
    interface DnnSecurityCenter {
        /**
          * The Dnn module id, required in order to access web services.
         */
        "moduleId": number;
    }
}
declare global {
    interface HTMLDnnSecurityCenterElement extends Components.DnnSecurityCenter, HTMLStencilElement {
    }
    var HTMLDnnSecurityCenterElement: {
        prototype: HTMLDnnSecurityCenterElement;
        new (): HTMLDnnSecurityCenterElement;
    };
    interface HTMLElementTagNameMap {
        "dnn-security-center": HTMLDnnSecurityCenterElement;
    }
}
declare namespace LocalJSX {
    interface DnnSecurityCenter {
        /**
          * The Dnn module id, required in order to access web services.
         */
        "moduleId": number;
    }
    interface IntrinsicElements {
        "dnn-security-center": DnnSecurityCenter;
    }
}
export { LocalJSX as JSX };
declare module "@stencil/core" {
    export namespace JSX {
        interface IntrinsicElements {
            "dnn-security-center": LocalJSX.DnnSecurityCenter & JSXBase.HTMLAttributes<HTMLDnnSecurityCenterElement>;
        }
    }
}
