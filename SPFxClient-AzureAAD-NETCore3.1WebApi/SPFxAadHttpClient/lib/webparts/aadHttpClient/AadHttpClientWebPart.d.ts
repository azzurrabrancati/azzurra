import { Version } from '@microsoft/sp-core-library';
import { IPropertyPaneConfiguration } from '@microsoft/sp-property-pane';
import { BaseClientSideWebPart } from '@microsoft/sp-webpart-base';
export interface IAadHttpClientWebPartProps {
    description: string;
}
export default class AadHttpClientWebPart extends BaseClientSideWebPart<IAadHttpClientWebPartProps> {
    render(): void;
    protected readonly dataVersion: Version;
    protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration;
}
//# sourceMappingURL=AadHttpClientWebPart.d.ts.map