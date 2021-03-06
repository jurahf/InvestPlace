﻿using Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.FaqService
{
    public class FaqService : IFaqService
    {
        public List<FaqDto> GetAll()
        {
            return new List<FaqDto>()
            {
                new FaqDto() { Order = 0,  Question = "Вносить собственные данные обязательно или нет?", Answer = "Данные необходимо указать верные для того что бы вы получали свои товары и деньги." },
                new FaqDto() { Order = 1,  Question = "После сбора моей картинки, я получаю товар или деньги?", Answer = "Вы получаете либо товар, либо 80% от стоимости товара на вашу банковскую карту или банковский счет. Это на ваше усмотрение" },
                new FaqDto() { Order = 2,  Question = "Для выставления своей картинки я должен/на оплатить сразу 50% от стоимости или могу собирать эту сумму постепенно покупая пазлы?", Answer = "На момент выставления своей картинки, в вашем кошельке отображается ваша оказанная помощь, которая должна составлять 50% от стоимости вашей картинки. Этими баллами и можете воспользоваться. Вы можете помогать в сборе другим участникам, а у вас в кошельке будут копиться баллы за оказанную помощь. Если балов не достаточно то необходимо докупить пазлов у других участников." },
                new FaqDto() { Order = 3,  Question = "Как я пойму, что стал обладателем товара после покупки пазла?", Answer = "Во вкладке Обмен/Продажа будут отображаться ваши собранные картинки, которые вы сможете продать или обменять на реальный товар." },
                new FaqDto() { Order = 4,  Question = "Могу ли я покупать свои пазлы?", Answer = "Нет, данная функция не предусмотрена." },
                new FaqDto() { Order = 5,  Question = "Что делать, если мой товар не выставляется на площадку?", Answer = "Если товар не выставляется, значит он не является уникальным, т.е. такой товар уже выставил другой пользователь." },
                new FaqDto() { Order = 6,  Question = "Как я могу вывести деньги на карту?", Answer = "Через вкладку “Кошелек”. Нажимаете вывод денег и через бегунок выставляете сумму, которую хотите вывести. Просим обратить внимание, что вывести и внести сумму можно кратную 500 рублей." },
                new FaqDto() { Order = 7,  Question = "Что делать, если человек не подтверждает перевод и не отвечает на звонки?", Answer = "Мы будем отслеживать этот процесс и решать эту проблему в кротчайшие сроки. Время на подтверждение операции - 24 часа." },
                new FaqDto() { Order = 8,  Question = "Как я получу товар, после сбора картинки?", Answer = "Товар отправляется по  адресу, который вы указали в аккаунте, службой доставки." },
                new FaqDto() { Order = 9,  Question = "Сколько максимум товаров я могу выставить в каждом диапазоне цены?", Answer = "Неограниченно." },
                new FaqDto() { Order = 10, Question = "Есть ли ограничения по покупке пазлов у других пользователей?", Answer = "Ограничений нет." },
                new FaqDto() { Order = 11, Question = "Почему цены на сайте иногда отличаются от цен сторонних сайтов?", Answer = "К сожалению время идет цены меняются, но не волнуйтесь в любом случае мы выполним все возложенные на нас условия в независимости подорожал товар или подешевел." },
            };
        }
    }
}
