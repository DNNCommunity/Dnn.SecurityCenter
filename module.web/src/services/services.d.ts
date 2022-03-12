export declare class ClientBase {
    private sf;
    private moduleId;
    constructor(configuration: ConfigureRequest);
    protected getBaseUrl(_defaultUrl: string, baseUrl?: string): string;
    protected transformOptions(options: RequestInit): Promise<RequestInit>;
}
export declare class ItemClient extends ClientBase {
    private http;
    private baseUrl;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    constructor(configuration: ConfigureRequest, baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });
    /**
     * Creates a new item.
     * @param item (optional) The item to create.
     * @return OK
     */
    createItem(item: CreateItemDTO | null | undefined, signal?: AbortSignal | undefined): Promise<ItemViewModel>;
    protected processCreateItem(response: Response): Promise<ItemViewModel>;
    /**
     * Gets a paged and sorted list of items matching a certain query.
     * @param query (optional) Gets or sets the optional search query.
     * @param page (optional) Gets or sets the page number to get.
     * @param pageSize (optional) Gets or sets the size of pages.
     * @param descending (optional) Gets or sets a value indicating whether the items should be ordered descending.
     * @return OK
     */
    getItemsPage(query: string | null | undefined, page: number | undefined, pageSize: number | undefined, descending: boolean | undefined, signal?: AbortSignal | undefined): Promise<ItemsPageViewModel>;
    protected processGetItemsPage(response: Response): Promise<ItemsPageViewModel>;
    /**
     * Deletes an existing item.
     * @param itemId The id of the item to delete.
     * @return OK
     */
    deleteItem(itemId: number, signal?: AbortSignal | undefined): Promise<void>;
    protected processDeleteItem(response: Response): Promise<void>;
    /**
     * Checks if a user can edit the current items.
     * @return OK
     */
    userCanEdit(signal?: AbortSignal | undefined): Promise<boolean>;
    protected processUserCanEdit(response: Response): Promise<boolean>;
    /**
     * Updates an existing item.
     * @param item (optional) The new information about the item, UpdateItemDTO.
     * @return OK
     */
    updateItem(item: UpdateItemDTO | null | undefined, signal?: AbortSignal | undefined): Promise<void>;
    protected processUpdateItem(response: Response): Promise<void>;
}
export declare class LocalizationClient extends ClientBase {
    private http;
    private baseUrl;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    constructor(configuration: ConfigureRequest, baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });
    /**
     * Gets localization keys and values.
     * @return OK
     */
    getLocalization(signal?: AbortSignal | undefined): Promise<LocalizationViewModel>;
    protected processGetLocalization(response: Response): Promise<LocalizationViewModel>;
}
export declare class SecurityClient extends ClientBase {
    private http;
    private baseUrl;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    constructor(configuration: ConfigureRequest, baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });
    /**
     * Gets all the DNN security bulletins.
     * @param versionString The version for which to get the security bulletins for in the format 090202 for v9.9.2.
     * @return A list of DNN security bulletins.
     */
    getSecurityBulletins(versionString: string | null, signal?: AbortSignal | undefined): Promise<SecurityBulletinsViewModel>;
    protected processGetSecurityBulletins(response: Response): Promise<SecurityBulletinsViewModel>;
}
/** Represents the basic information about an item. */
export declare class ItemViewModel implements IItemViewModel {
    /** Gets or sets the id of the item. */
    id: number;
    /** Gets or sets the name of the item. */
    name: string;
    /** Gets or sets the item description. */
    description?: string | undefined;
    constructor(data?: IItemViewModel);
    init(_data?: any): void;
    static fromJS(data: any): ItemViewModel;
    toJSON(data?: any): any;
}
/** Represents the basic information about an item. */
export interface IItemViewModel {
    /** Gets or sets the id of the item. */
    id: number;
    /** Gets or sets the name of the item. */
    name: string;
    /** Gets or sets the item description. */
    description?: string | undefined;
}
export declare class Exception implements IException {
    message?: string | undefined;
    innerException?: Exception | undefined;
    stackTrace?: string | undefined;
    source?: string | undefined;
    constructor(data?: IException);
    init(_data?: any): void;
    static fromJS(data: any): Exception;
    toJSON(data?: any): any;
}
export interface IException {
    message?: string | undefined;
    innerException?: Exception | undefined;
    stackTrace?: string | undefined;
    source?: string | undefined;
}
/** Data transfer object to create a new item. */
export declare class CreateItemDTO implements ICreateItemDTO {
    /** Gets or sets the name for the item. */
    name: string;
    /** Gets or sets the description of the item. */
    description?: string | undefined;
    constructor(data?: ICreateItemDTO);
    init(_data?: any): void;
    static fromJS(data: any): CreateItemDTO;
    toJSON(data?: any): any;
}
/** Data transfer object to create a new item. */
export interface ICreateItemDTO {
    /** Gets or sets the name for the item. */
    name: string;
    /** Gets or sets the description of the item. */
    description?: string | undefined;
}
/** Represents a page of items, Item. */
export declare class ItemsPageViewModel implements IItemsPageViewModel {
    /** Gets or sets a list of items for this page. */
    items?: ItemViewModel[] | undefined;
    /** Gets or sets the current page number. */
    page: number;
    /** Gets or sets the total amount of results found. */
    resultCount: number;
    /** Gets or sets the total amount of pages available. */
    pageCount: number;
    constructor(data?: IItemsPageViewModel);
    init(_data?: any): void;
    static fromJS(data: any): ItemsPageViewModel;
    toJSON(data?: any): any;
}
/** Represents a page of items, Item. */
export interface IItemsPageViewModel {
    /** Gets or sets a list of items for this page. */
    items?: ItemViewModel[] | undefined;
    /** Gets or sets the current page number. */
    page: number;
    /** Gets or sets the total amount of results found. */
    resultCount: number;
    /** Gets or sets the total amount of pages available. */
    pageCount: number;
}
export declare class SystemException extends Exception implements ISystemException {
    constructor(data?: ISystemException);
    init(_data?: any): void;
    static fromJS(data: any): SystemException;
    toJSON(data?: any): any;
}
export interface ISystemException extends IException {
}
export declare class ArgumentException extends SystemException implements IArgumentException {
    message?: string | undefined;
    paramName?: string | undefined;
    constructor(data?: IArgumentException);
    init(_data?: any): void;
    static fromJS(data: any): ArgumentException;
    toJSON(data?: any): any;
}
export interface IArgumentException extends ISystemException {
    message?: string | undefined;
    paramName?: string | undefined;
}
/** Data transfer object used to update an item. */
export declare class UpdateItemDTO extends CreateItemDTO implements IUpdateItemDTO {
    /** Gets or sets the id of the item to edit. */
    id: number;
    constructor(data?: IUpdateItemDTO);
    init(_data?: any): void;
    static fromJS(data: any): UpdateItemDTO;
    toJSON(data?: any): any;
}
/** Data transfer object used to update an item. */
export interface IUpdateItemDTO extends ICreateItemDTO {
    /** Gets or sets the id of the item to edit. */
    id: number;
}
/** A viewmodel that exposes all resource keys in strong types. */
export declare class LocalizationViewModel implements ILocalizationViewModel {
    /** Localized strings present the ModelValidation resources. */
    modelValidation?: ModelValidationInfo | undefined;
    /** Localized strings present the UI resources. */
    uI?: UIInfo | undefined;
    constructor(data?: ILocalizationViewModel);
    init(_data?: any): void;
    static fromJS(data: any): LocalizationViewModel;
    toJSON(data?: any): any;
}
/** A viewmodel that exposes all resource keys in strong types. */
export interface ILocalizationViewModel {
    /** Localized strings present the ModelValidation resources. */
    modelValidation?: ModelValidationInfo | undefined;
    /** Localized strings present the UI resources. */
    uI?: UIInfo | undefined;
}
/** Localized strings for the ModelValidation resources. */
export declare class ModelValidationInfo implements IModelValidationInfo {
    /** Gets or sets the IdGreaterThanZero localized text. */
    idGreaterThanZero?: string | undefined;
    /** Gets or sets the NameRequired localized text. */
    nameRequired?: string | undefined;
    constructor(data?: IModelValidationInfo);
    init(_data?: any): void;
    static fromJS(data: any): ModelValidationInfo;
    toJSON(data?: any): any;
}
/** Localized strings for the ModelValidation resources. */
export interface IModelValidationInfo {
    /** Gets or sets the IdGreaterThanZero localized text. */
    idGreaterThanZero?: string | undefined;
    /** Gets or sets the NameRequired localized text. */
    nameRequired?: string | undefined;
}
/** Localized strings for the UI resources. */
export declare class UIInfo implements IUIInfo {
    /** Gets or sets the DnnPlatformVersion localized text. */
    dnnPlatformVersion?: string | undefined;
    /** Gets or sets the DnnSecurityCenter localized text. */
    dnnSecurityCenter?: string | undefined;
    constructor(data?: IUIInfo);
    init(_data?: any): void;
    static fromJS(data: any): UIInfo;
    toJSON(data?: any): any;
}
/** Localized strings for the UI resources. */
export interface IUIInfo {
    /** Gets or sets the DnnPlatformVersion localized text. */
    dnnPlatformVersion?: string | undefined;
    /** Gets or sets the DnnSecurityCenter localized text. */
    dnnSecurityCenter?: string | undefined;
}
/** A viewmodel that represents DNN Security Bulletins. */
export declare class SecurityBulletinsViewModel implements ISecurityBulletinsViewModel {
    /** Gets or sets the title of the RSS feed. */
    title?: string | undefined;
    /** Gets or sets the url to download DNN Platform. */
    link?: string | undefined;
    /** Gets or sets the RSS feed description. */
    description?: string | undefined;
    /** Gets or sets the list of security bulletins. */
    bulletins?: Bulletin[] | undefined;
    constructor(data?: ISecurityBulletinsViewModel);
    init(_data?: any): void;
    static fromJS(data: any): SecurityBulletinsViewModel;
    toJSON(data?: any): any;
}
/** A viewmodel that represents DNN Security Bulletins. */
export interface ISecurityBulletinsViewModel {
    /** Gets or sets the title of the RSS feed. */
    title?: string | undefined;
    /** Gets or sets the url to download DNN Platform. */
    link?: string | undefined;
    /** Gets or sets the RSS feed description. */
    description?: string | undefined;
    /** Gets or sets the list of security bulletins. */
    bulletins?: Bulletin[] | undefined;
}
/** Represents a single DNN Security Bulletin. */
export declare class Bulletin implements IBulletin {
    /** Gets or sets a link to the detailed security bulletin. */
    link?: string | undefined;
    /** Gets or sets the title of the bulletin. */
    title?: string | undefined;
    /** Gets or sets the short description of the bulletin. */
    description?: string | undefined;
    /** Gets or sets a string representing the date of announcement. */
    publicationDateUtc: Date;
    constructor(data?: IBulletin);
    init(_data?: any): void;
    static fromJS(data: any): Bulletin;
    toJSON(data?: any): any;
}
/** Represents a single DNN Security Bulletin. */
export interface IBulletin {
    /** Gets or sets a link to the detailed security bulletin. */
    link?: string | undefined;
    /** Gets or sets the title of the bulletin. */
    title?: string | undefined;
    /** Gets or sets the short description of the bulletin. */
    description?: string | undefined;
    /** Gets or sets a string representing the date of announcement. */
    publicationDateUtc: Date;
}
export declare class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: {
        [key: string]: any;
    };
    result: any;
    constructor(message: string, status: number, response: string, headers: {
        [key: string]: any;
    }, result: any);
    protected isApiException: boolean;
    static isApiException(obj: any): obj is ApiException;
}
export interface ConfigureRequest {
    moduleId: number;
}
