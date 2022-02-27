import { Component, h, Prop, Host, Element, State } from "@stencil/core";
import { LocalizationClient, SecurityClient, LocalizationViewModel, SecurityBulletinsViewModel } from "../../services/services";
import state, { localizationState } from "../../store/state";
import alertError from "../../services/alert-error";
import dnnVersions from "../../data/dnn-versions";

@Component({
  tag: 'dnn-security-center',
  styleUrl: 'dnn-security-center.scss',
  shadow: true
})
export class DnnSecurityCenter {
  private localizationClient: LocalizationClient;
  private securityClient: SecurityClient;
  private resx: LocalizationViewModel;
  
  constructor() {
    state.moduleId = this.moduleId;
    this.localizationClient = new LocalizationClient({ moduleId: this.moduleId });
    this.securityClient = new SecurityClient({ moduleId: this.moduleId });
  }
  
  @Element() el: HTMLDnnSecurityCenterElement;

  /** The Dnn module id, required in order to access web services. */
  @Prop() moduleId!: number;

  @State() selectValue: string = '090101';

  @State() securityBulletins: SecurityBulletinsViewModel;

  componentWillLoad() {
    return new Promise<void>((resolve, reject) => {
      this.localizationClient.getLocalization()
        .then(l => {
          localizationState.viewModel = l;
          this.resx = localizationState.viewModel;
          resolve();
        })
        .catch(reason => {
          alertError(reason);
          reject();
        });
    });
  }

  componentDidLoad() {
    this.getSecurityBulletins();
  }

  private getSecurityBulletins() {
    this.securityClient.getSecurityBulletins(this.selectValue).then(data => {
      this.securityBulletins = data;
    }).catch(reason => {
      alertError(reason);
    });
  }

  private handleSelect(event): void {
    console.log(event.target.value + ' selected');
    this.selectValue = event.target.value;
    this.getSecurityBulletins();
  }

  private decodeHtml(text: string): string {
    return text.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace('”', '').replace('”', '');
  }

  render() {
    return <Host>
      <div>
        <h1>{this.resx.uI.dnnSecurityCenter}</h1>
        <div>
          {this.resx.uI.dnnPlatformVersion}: &nbsp;
          <select name="dnnVersions" onInput={(e: any) => this.handleSelect(e)}>
            {dnnVersions.map(version => 
              <option value={version.replace(/\./g, '')}>{version}</option>
            )}
          </select>
        </div>
        {this.securityBulletins?.bulletins?.map((bulletin) => {
          return (
            <div class="item">
              <h2 class="item-title">{bulletin.title}</h2>
              <h4 class="item-published">Published: {bulletin.publicationDateUtc.toLocaleDateString()}</h4>
              <div class="item-description" innerHTML={this.decodeHtml(bulletin.description)}></div>
            </div>
          )
        })}
      </div>
    </Host>;
  }
}
