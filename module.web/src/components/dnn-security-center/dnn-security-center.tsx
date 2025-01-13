import { Component, h, Prop, Host, Element, State, Watch } from "@stencil/core";
import { LocalizationClient, SecurityClient, LocalizationViewModel, SecurityBulletinsViewModel } from "../../services/services";
import state, { localizationState } from "../../store/state";
import alertError from "../../services/alert-error";
import GithubService from "../../services/github";
import preGitHubDnnVersions from "../../data/pre-github-dnn-versions";
import { Debounce } from "@dnncommunity/dnn-elements";

@Component({
  tag: 'dnn-security-center',
  styleUrl: 'dnn-security-center.scss',
  shadow: true
})
export class DnnSecurityCenter {
  private localizationClient: LocalizationClient;
  private securityClient: SecurityClient;
  private resx: LocalizationViewModel;
  private githubService: GithubService;

  constructor() {
    state.moduleId = this.moduleId;
    this.localizationClient = new LocalizationClient({ moduleId: this.moduleId });
    this.securityClient = new SecurityClient({ moduleId: this.moduleId });
    this.githubService = new GithubService();
  }
  
  @Element() el: HTMLDnnSecurityCenterElement;
  
  /** The Dnn module id, required in order to access web services. */
  @Prop() moduleId!: number;
  
  @State() securityBulletins: SecurityBulletinsViewModel;
  @State() expandedBulletinIndex: number;
  @State() dnnVersions: string[];
  @State() selectValue: string;
  @Watch('selectValue')
  selectValueChanged(newValue: string, oldValue: string) {
    if (newValue !== oldValue) {
      this.getSecurityBulletins();
    }
  }

  componentWillLoad() {
    return new Promise<void>((resolve, reject) => {
      this.getDnnTags();
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

  private getDnnTags() {
    this.githubService.getTags().then(data => {
      this.dnnVersions = data;
      this.dnnVersions = [...this.dnnVersions, ...preGitHubDnnVersions]
      this.selectValue = this.dnnVersions[0].replace(/\./g, '');
    }).catch(reason => {
      alertError(reason);
    });
  }
  
  private getSecurityBulletins() {
    this.securityClient.getSecurityBulletins(this.selectValue).then(data => {
      this.securityBulletins = data;
    }).catch(reason => {
      alertError(reason);
    });
  }

  private decodeHtml(text: string): string {
    return text.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace('”', '').replace('”', '');
  }

  @Debounce(200)
  private toggleCollapse(event: CustomEvent, index: number): void {
    const el = this.el.shadowRoot.querySelector(`#dnn-chevron-${index}`);
    requestAnimationFrame(() => {

      if (this.expandedBulletinIndex === index) {
        this.expandedBulletinIndex = undefined;
        el.removeAttribute('expanded');
      } else {
        if (event.detail) {
          this.expandedBulletinIndex = index;
        }
      }
    });
  }

  render() {
    return <Host>
      <div>
        <h1>{this.resx.uI.dnnSecurityCenter}</h1>
        {this.dnnVersions &&
          <h3>
            {this.resx.uI.dnnPlatformVersion}: &nbsp;
            <dnn-select name="dnnVersions" onValueChange={e => this.selectValue = e.detail}>
              {this.dnnVersions.map(version => 
                <option value={version.replace(/\./g, '')} selected={this.selectValue == version.replace(/\./g, '')}>{version}</option>
              )}
            </dnn-select>
          </h3>
        }
        {this.securityBulletins === undefined &&
          <div class="loading">{this.resx.uI.loading}</div>
        }
        {this.securityBulletins?.bulletins?.length === 0 &&
          <div class="no-bulletins">{this.resx.uI.noBulletins}</div>
        }
        {this.securityBulletins?.bulletins?.map((bulletin, index) => {
          return (
            <div class="bulletins">
              <div class="collapse-row">
                <div class="collapse-title">
                  <dnn-chevron id={'dnn-chevron-' + index} 
                    onChanged={e => {
                      this.toggleCollapse(e, index);
                    }}
                    expanded={this.expandedBulletinIndex === index}
                  ></dnn-chevron>
                  <h2 class="item-title">{bulletin.title}</h2>
                  <div class="item-published">Published: <strong>{bulletin.publicationDateUtc.toLocaleDateString()}</strong></div>
                </div>
              </div>
              <dnn-collapsible id="dnn-collapsible1" transition-duration="150" expanded={this.expandedBulletinIndex == index}>
                <div class="collapsible-slot-content item-description" innerHTML={this.decodeHtml(bulletin.description)}></div>
              </dnn-collapsible>
            </div>
          )
        })}
      </div>
    </Host>;
  }
}
