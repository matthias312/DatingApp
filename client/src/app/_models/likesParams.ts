export class LikesParams{
    predicate: string;
    pageNumber = 1;
    pageSize = 5;

    constructor(predicate: string, pageNumber: number, pageSize: number){
        this.predicate = predicate;
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
    }
}

