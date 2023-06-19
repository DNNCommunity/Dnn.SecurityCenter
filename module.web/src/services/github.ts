import { Octokit } from "@octokit/core";
import { paginateRest } from "@octokit/plugin-paginate-rest";

export default class GithubService {
    private octokit: Octokit;
    
    constructor() {
        this.octokit = new Octokit();
    }
    
    async getTags() {
        let tags = await paginateRest(this.octokit).paginate("GET /repos/dnnsoftware/Dnn.Platform/tags", {
            accept: "application/vnd.github.v3+json",
            owner: "dnnsoftware",
            repo: "Dnn.Platform"
        });
        tags = tags.map((tag: any) => tag.name.replace("v", ""));
        tags = tags.filter((tag: any) => tag.match(/^\d+\.\d+\.\d+$/));
        const returnTags = tags.map((tag: any) => tag.split(".")[0].padStart(2, "0") + "." + tag.split(".")[1].padStart(2, "0") + "." + tag.split(".")[2].padStart(2, "0"));
        return returnTags;
    }

}