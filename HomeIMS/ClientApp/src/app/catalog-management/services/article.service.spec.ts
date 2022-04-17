import { TestBed } from '@angular/core/testing';
import {
  ApolloTestingModule,
  ApolloTestingController,
} from 'apollo-angular/testing';

import { ArticleService } from './article.service';

describe('ArticleService', () => {
  let service: ArticleService;

  let controller: ApolloTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ArticleService],
      imports: [ApolloTestingModule]
    });

    controller = TestBed.inject(ApolloTestingController);

    service = TestBed.inject(ArticleService);
  });

  afterEach(() => {
    controller.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // it('provides empty articles on error', () => {
  //   service.getArticles().subscribe((articles) => {
  //     expect(articles).toHaveSize(0);
  //   });

  //   let operation = controller.expectOne('getArticles');

  //   operation.flush();
  // });

  it('provides empty articles correctly', () => {
    let expectedData = {
      data: {
        articles: [],
      },
    };

    service.getArticles().subscribe((articles) => {
      console.log(articles);
      expect(articles).toHaveSize(0);
    });

    let operation = controller.expectOne('getArticles');

    operation.flush(expectedData);
  });

  it('provides 1 article correctly', () => {
    let expectedData = {
      data: {
        articles: [
          {
            id: 42,
            ean: '12345678',
            description: 'TestArticle',
          }
        ],
      },
    };

    service.getArticles().subscribe((articles) => {
      expect(articles[0].id).toEqual(expectedData.data.articles[0].id);
      expect(articles[0].ean).toEqual(expectedData.data.articles[0].ean);
      expect(articles[0].description).toEqual(expectedData.data.articles[0].description);
    });

    let operation = controller.expectOne('getArticles');

    operation.flush(expectedData);
  });

  it('provides 3 articles correctly', () => {
    let expectedData = {
      data: {
        articles: [
          {
            id: 0,
            ean: '12345678',
            description: 'TestArticle',
          },
          {
            id: 23,
            ean: '87654321',
            description: 'Bla',
          },
          {
            id: 42,
            ean: '11235813',
            description: 'Blubb',
          }
        ],
      },
    };

    service.getArticles().subscribe((articles) => {
      expect(articles[0].id).toEqual(expectedData.data.articles[0].id);
      expect(articles[0].ean).toEqual(expectedData.data.articles[0].ean);
      expect(articles[0].description).toEqual(expectedData.data.articles[0].description);
      expect(articles[1].id).toEqual(expectedData.data.articles[1].id);
      expect(articles[1].ean).toEqual(expectedData.data.articles[1].ean);
      expect(articles[1].description).toEqual(expectedData.data.articles[1].description);
      expect(articles[2].id).toEqual(expectedData.data.articles[2].id);
      expect(articles[2].ean).toEqual(expectedData.data.articles[2].ean);
      expect(articles[2].description).toEqual(expectedData.data.articles[2].description);
    });

    let operation = controller.expectOne('getArticles');

    operation.flush(expectedData);
  });
});
