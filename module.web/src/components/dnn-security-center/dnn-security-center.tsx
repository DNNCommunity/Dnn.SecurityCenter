import { Component, h, Prop, Host, Element, State } from "@stencil/core";
import { LocalizationClient, LocalizationViewModel } from "../../services/services";
import state, { localizationState } from "../../store/state";
import alertError from "../../services/alert-error";

@Component({
  tag: 'dnn-security-center',
  styleUrl: 'dnn-security-center.scss',
  shadow: true
})
export class DnnSecurityCenter {
  private localizationService: LocalizationClient;
  private resx: LocalizationViewModel;

  constructor() {
    state.moduleId = this.moduleId;
    this.localizationService = new LocalizationClient({ moduleId: this.moduleId });
  }

  @Element() el: HTMLDnnSecurityCenterElement;

  /** The Dnn module id, required in order to access web services. */
  @Prop() moduleId!: number;

  @State() selectValue: string;

  componentWillLoad() {
    const localizationPromise = new Promise<void>((resolve, reject) => {
      this.localizationService.getLocalization()
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
    
    const rssPromise = new Promise<void>((resolve) => {
      // TODO: Use a web service to get the RSS feed.
      resolve();
    });

    return Promise.all([localizationPromise, rssPromise]);
  }

  private handleSelect(event): void {
    console.log(event.target.value);
    this.selectValue = event.target.value;
  }

  render() {
    return <Host>
      <div>
        <h1>{this.resx.uI.dnnSecurityCenter}</h1>
        <div>
          {this.resx.uI.dnnPlatformVersion}: &nbsp;
          <select name="dnnVersions" onInput={(event) => this.handleSelect(event)} id="dnnVersions" class="NormalTextBox">
            <option selected={true} value="<All Versions>">&lt;All Versions&gt;</option>
            <option value="091001" selected={this.selectValue === '091001'}>09.10.01</option>
            <option value="091000" selected={this.selectValue === '091000'}>09.10.00</option>
            <option value="090901" selected={this.selectValue === '090901'}>09.09.01</option>
            <option value="090900" selected={this.selectValue === '090900'}>09.09.00</option>
            <option value="090801" selected={this.selectValue === '090801'}>09.08.01</option>
            <option value="090800" selected={this.selectValue === '090800'}>09.08.00</option>
            <option value="090702" selected={this.selectValue === '090702'}>09.07.02</option>
            <option value="090701" selected={this.selectValue === '090701'}>09.07.01</option>
            <option value="090700" selected={this.selectValue === '090700'}>09.07.00</option>
            <option value="090602" selected={this.selectValue === '090602'}>09.06.02</option>
            <option value="090601" selected={this.selectValue === '090601'}>09.06.01</option>
            <option value="090600" selected={this.selectValue === '090600'}>09.06.00</option>
            <option value="090500" selected={this.selectValue === '090500'}>09.05.00</option>
            <option value="090404" selected={this.selectValue === '090404'}>09.04.04</option>
            <option value="090403" selected={this.selectValue === '090403'}>09.04.03</option>
            <option value="090402" selected={this.selectValue === '090402'}>09.04.02</option>
            <option value="090401" selected={this.selectValue === '090401'}>09.04.01</option>
            <option value="090400" selected={this.selectValue === '090400'}>09.04.00</option>
            <option value="090302" selected={this.selectValue === '090302'}>09.03.02</option>
            <option value="090301" selected={this.selectValue === '090301'}>09.03.01</option>
            <option value="090202" selected={this.selectValue === '090202'}>09.02.02</option>
            <option value="090201" selected={this.selectValue === '090201'}>09.02.01</option>
            <option value="090200" selected={this.selectValue === '090200'}>09.02.00</option>
            <option value="090101" selected={this.selectValue === '090101'}>09.01.01</option>
            <option value="090100" selected={this.selectValue === '090100'}>09.01.00</option>
            <option value="090002" selected={this.selectValue === '090002'}>09.00.02</option>
            <option value="090001" selected={this.selectValue === '090001'}>09.00.01</option>
            <option value="090000" selected={this.selectValue === '090000'}>09.00.00</option>
            <option value="080004" selected={this.selectValue === '080004'}>08.00.04</option>
            <option value="080003" selected={this.selectValue === '080003'}>08.00.03</option>
            <option value="080002" selected={this.selectValue === '080002'}>08.00.02</option>
            <option value="080001" selected={this.selectValue === '080001'}>08.00.01</option>
            <option value="080000" selected={this.selectValue === '080000'}>08.00.00</option>
            <option value="070402" selected={this.selectValue === '070402'}>07.04.02</option>
            <option value="070401" selected={this.selectValue === '070401'}>07.04.01</option>
            <option value="070400" selected={this.selectValue === '070400'}>07.04.00</option>
            <option value="070304" selected={this.selectValue === '070304'}>07.03.04</option>
            <option value="070303" selected={this.selectValue === '070303'}>07.03.03</option>
            <option value="070302" selected={this.selectValue === '070302'}>07.03.02</option>
            <option value="070301" selected={this.selectValue === '070301'}>07.03.01</option>
            <option value="070300" selected={this.selectValue === '070300'}>07.03.00</option>
            <option value="070202" selected={this.selectValue === '070202'}>07.02.02</option>
            <option value="070201" selected={this.selectValue === '070201'}>07.02.01</option>
            <option value="070200" selected={this.selectValue === '070200'}>07.02.00</option>
            <option value="070102" selected={this.selectValue === '070102'}>07.01.02</option>
            <option value="070101" selected={this.selectValue === '070101'}>07.01.01</option>
            <option value="070100" selected={this.selectValue === '070100'}>07.01.00</option>
            <option value="070006" selected={this.selectValue === '070006'}>07.00.06</option>
            <option value="070005" selected={this.selectValue === '070005'}>07.00.05</option>
            <option value="070004" selected={this.selectValue === '070004'}>07.00.04</option>
            <option value="070003" selected={this.selectValue === '070003'}>07.00.03</option>
            <option value="070002" selected={this.selectValue === '070002'}>07.00.02</option>
            <option value="070001" selected={this.selectValue === '070001'}>07.00.01</option>
            <option value="070000" selected={this.selectValue === '070000'}>07.00.00</option>
            <option value="060209" selected={this.selectValue === '060209'}>06.02.09</option>
            <option value="060208" selected={this.selectValue === '060208'}>06.02.08</option>
            <option value="060207" selected={this.selectValue === '060207'}>06.02.07</option>
            <option value="060206" selected={this.selectValue === '060206'}>06.02.06</option>
            <option value="060205" selected={this.selectValue === '060205'}>06.02.05</option>
            <option value="060204" selected={this.selectValue === '060204'}>06.02.04</option>
            <option value="060203" selected={this.selectValue === '060203'}>06.02.03</option>
            <option value="060202" selected={this.selectValue === '060202'}>06.02.02</option>
            <option value="060201" selected={this.selectValue === '060201'}>06.02.01</option>
            <option value="060200" selected={this.selectValue === '060200'}>06.02.00</option>
            <option value="060105" selected={this.selectValue === '060105'}>06.01.05</option>
            <option value="060104" selected={this.selectValue === '060104'}>06.01.04</option>
            <option value="060103" selected={this.selectValue === '060103'}>06.01.03</option>
            <option value="060102" selected={this.selectValue === '060102'}>06.01.02</option>
            <option value="060101" selected={this.selectValue === '060101'}>06.01.01</option>
            <option value="060100" selected={this.selectValue === '060100'}>06.01.00</option>
            <option value="060002" selected={this.selectValue === '060002'}>06.00.02</option>
            <option value="060001" selected={this.selectValue === '060001'}>06.00.01</option>
            <option value="060000" selected={this.selectValue === '060000'}>06.00.00</option>
            <option value="050608" selected={this.selectValue === '050608'}>05.06.08</option>
            <option value="050607" selected={this.selectValue === '050607'}>05.06.07</option>
            <option value="050606" selected={this.selectValue === '050606'}>05.06.06</option>
            <option value="050605" selected={this.selectValue === '050605'}>05.06.05</option>
            <option value="050604" selected={this.selectValue === '050604'}>05.06.04</option>
            <option value="050603" selected={this.selectValue === '050603'}>05.06.03</option>
            <option value="050602" selected={this.selectValue === '050602'}>05.06.02</option>
            <option value="050601" selected={this.selectValue === '050601'}>05.06.01</option>
            <option value="050600" selected={this.selectValue === '050600'}>05.06.00</option>
            <option value="050501" selected={this.selectValue === '050501'}>05.05.01</option>
            <option value="050500" selected={this.selectValue === '050500'}>05.05.00</option>
            <option value="050404" selected={this.selectValue === '050404'}>05.04.04</option>
            <option value="050403" selected={this.selectValue === '050403'}>05.04.03</option>
            <option value="050402" selected={this.selectValue === '050402'}>05.04.02</option>
            <option value="050401" selected={this.selectValue === '050401'}>05.04.01</option>
            <option value="050400" selected={this.selectValue === '050400'}>05.04.00</option>
            <option value="050301" selected={this.selectValue === '050301'}>05.03.01</option>
            <option value="050300" selected={this.selectValue === '050300'}>05.03.00</option>
            <option value="050203" selected={this.selectValue === '050203'}>05.02.03</option>
            <option value="050202" selected={this.selectValue === '050202'}>05.02.02</option>
            <option value="050201" selected={this.selectValue === '050201'}>05.02.01</option>
            <option value="050200" selected={this.selectValue === '050200'}>05.02.00</option>
            <option value="050104" selected={this.selectValue === '050104'}>05.01.04</option>
            <option value="050103" selected={this.selectValue === '050103'}>05.01.03</option>
            <option value="050102" selected={this.selectValue === '050102'}>05.01.02</option>
            <option value="050101" selected={this.selectValue === '050101'}>05.01.01</option>
            <option value="050100" selected={this.selectValue === '050100'}>05.01.00</option>
            <option value="050001" selected={this.selectValue === '050001'}>05.00.01</option>
            <option value="050000" selected={this.selectValue === '050000'}>05.00.00</option>
            <option value="040905" selected={this.selectValue === '040905'}>04.09.05</option>
            <option value="040904" selected={this.selectValue === '040904'}>04.09.04</option>
            <option value="040903" selected={this.selectValue === '040903'}>04.09.03</option>
            <option value="040902" selected={this.selectValue === '040902'}>04.09.02</option>
            <option value="040901" selected={this.selectValue === '040901'}>04.09.01</option>
            <option value="040900" selected={this.selectValue === '040900'}>04.09.00</option>
            <option value="040804" selected={this.selectValue === '040804'}>04.08.04</option>
            <option value="040803" selected={this.selectValue === '040803'}>04.08.03</option>
            <option value="040802" selected={this.selectValue === '040802'}>04.08.02</option>
            <option value="040801" selected={this.selectValue === '040801'}>04.08.01</option>
            <option value="040800" selected={this.selectValue === '040800'}>04.08.00</option>
            <option value="040700" selected={this.selectValue === '040700'}>04.07.00</option>
            <option value="040602" selected={this.selectValue === '040602'}>04.06.02</option>
            <option value="040601" selected={this.selectValue === '040601'}>04.06.01</option>
            <option value="040600" selected={this.selectValue === '040600'}>04.06.00</option>
            <option value="040505" selected={this.selectValue === '040505'}>04.05.05</option>
            <option value="040504" selected={this.selectValue === '040504'}>04.05.04</option>
            <option value="040503" selected={this.selectValue === '040503'}>04.05.03</option>
            <option value="040502" selected={this.selectValue === '040502'}>04.05.02</option>
            <option value="040501" selected={this.selectValue === '040501'}>04.05.01</option>
            <option value="040500" selected={this.selectValue === '040500'}>04.05.00</option>
            <option value="040401" selected={this.selectValue === '040401'}>04.04.01</option>
            <option value="040400" selected={this.selectValue === '040400'}>04.04.00</option>
            <option value="040307" selected={this.selectValue === '040307'}>04.03.07</option>
            <option value="040306" selected={this.selectValue === '040306'}>04.03.06</option>
            <option value="040305" selected={this.selectValue === '040305'}>04.03.05</option>
            <option value="040304" selected={this.selectValue === '040304'}>04.03.04</option>
            <option value="040303" selected={this.selectValue === '040303'}>04.03.03</option>
            <option value="040302" selected={this.selectValue === '040302'}>04.03.02</option>
            <option value="040301" selected={this.selectValue === '040301'}>04.03.01</option>
            <option value="040300" selected={this.selectValue === '040300'}>04.03.00</option>
            <option value="040002" selected={this.selectValue === '040002'}>04.00.02</option>
            <option value="040001" selected={this.selectValue === '040001'}>04.00.01</option>
            <option value="040000" selected={this.selectValue === '040000'}>04.00.00</option>
            <option value="030307" selected={this.selectValue === '030307'}>03.03.07</option>
            <option value="030306" selected={this.selectValue === '030306'}>03.03.06</option>
            <option value="030305" selected={this.selectValue === '030305'}>03.03.05</option>
            <option value="030304" selected={this.selectValue === '030304'}>03.03.04</option>
            <option value="030303" selected={this.selectValue === '030303'}>03.03.03</option>
            <option value="030302" selected={this.selectValue === '030302'}>03.03.02</option>
            <option value="030301" selected={this.selectValue === '030301'}>03.03.01</option>
            <option value="030300" selected={this.selectValue === '030300'}>03.03.00</option>
            <option value="030206" selected={this.selectValue === '030206'}>03.02.06</option>
            <option value="030204" selected={this.selectValue === '030204'}>03.02.04</option>
            <option value="030203" selected={this.selectValue === '030203'}>03.02.03</option>
            <option value="030202" selected={this.selectValue === '030202'}>03.02.02</option>
            <option value="030201" selected={this.selectValue === '030201'}>03.02.01</option>
            <option value="030200" selected={this.selectValue === '030200'}>03.02.00</option>
            <option value="030101" selected={this.selectValue === '030101'}>03.01.01</option>
            <option value="030100" selected={this.selectValue === '030100'}>03.01.00</option>
          </select>
        </div>
        <div class="feed"></div>
      </div>
    </Host>;
  }
}
