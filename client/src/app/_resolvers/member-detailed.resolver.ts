import { ActivatedRouteSnapshot, Resolve } from "@angular/router";
import { Member } from "../_models/member";
import { Observable } from 'rxjs';
import { Injectable } from "@angular/core";
import { MembersService } from "../_services/members.service";

@Injectable({
    providedIn: 'root'
})

export class MemberDetailedResolver implements Resolve<Member> {

    constructor(private memberservice: MembersService) {
    }

    resolve(route: ActivatedRouteSnapshot): Observable<Member> {
        return this.memberservice.getMember(route.paramMap.get("username"));
    }
}