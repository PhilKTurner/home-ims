import { Injectable } from '@angular/core';
import { Apollo, gql } from 'apollo-angular';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Article } from '../models/article';

@Injectable({
  providedIn: 'root'
  // TODO Why does providedIn CatalogManagementModule cause init errors in the module?
})
export class ArticleService {

  constructor(private apollo: Apollo) { }

  public getArticles(): Observable<Article[]> {
    const articleQuery = gql`
    query getArticles {
      articles {
        id,
        ean,
        description
      }
    }
  `

    return this.apollo
      .query<any>({
        query: articleQuery
      })
      .pipe(
        map<any, Article[]>(result => result.data.articles)
      );
  }
}
